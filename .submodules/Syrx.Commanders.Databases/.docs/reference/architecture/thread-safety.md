# Thread Safety

Thread-safety guarantees and concurrency model for Syrx.Commanders.Databases.

---

## Safety Summary

| Component | Thread-Safe? | Protection | Notes |
|-----------|-------------|-----------|-------|
| `DatabaseCommander<T>` | ✅ Yes | ConcurrentDictionary cache | Safe in transient, scoped, or singleton lifetimes |
| `DatabaseConnector` | ✅ Yes | ConcurrentDictionary cache | Safe in transient, scoped, or singleton lifetimes |
| `DatabaseCommandReader` | ✅ Yes | ConcurrentDictionary cache | Safe in transient, scoped, or singleton lifetimes |
| `ICommanderSettings` (sealed records) | ✅ Yes | Immutable | Share freely; no mutations |
| `IDbConnection` (from pool) | ⚠️ Per-Request | Not shared | One per thread/request |
| `DbTransaction` | ⚠️ Per-Request | Not shared | One per connection |

---

## Shared Components (Safe)

### DatabaseCommander<T>

Thread-safe runtime component:

```csharp
// Service lifetime may be transient, scoped, or singleton
// depending on which registration extension path is used.

// Multiple threads call simultaneously:
Task.Run(() => _commander.QueryAsync<User>());  // Thread 1
Task.Run(() => _commander.QueryAsync<User>());  // Thread 2
Task.Run(() => _commander.ExecuteAsync(user));  // Thread 3

// ✅ All safe. Each thread:
// ├─ Retrieves own connection from pool
// ├─ Shares command cache (thread-safe)
// └─ No data races
```

### Command Cache (ConcurrentDictionary)

```csharp
// Internal field in DatabaseCommander<T>:
private readonly ConcurrentDictionary<string, CommandSetting> _commandCache = new();

// At runtime with 100 concurrent threads:
// ├─ All call: await _commander.QueryAsync<User>(...)
// ├─ All attempt: _commandCache.GetOrAdd(key, loadFunc)
// ├─ First thread: Cache miss; loadFunc executes (LINQ query)
// ├─ Other 99 threads: Wait; first completes; re-use result
// └─ Result: One LINQ execution; all threads get same CommandSetting

// ✅ Zero data races. ConcurrentDictionary handles synchronization.
```

### Connection Alias Cache (ConcurrentDictionary)

Same as command cache - thread-safe under concurrent access.

### Configuration Records (Immutable)

All configuration records are sealed and immutable:

```csharp
public sealed record CommanderSettings : ICommanderSettings
{
    public required List<ConnectionStringSetting> Connections { get; init; }
    public required List<NamespaceSetting> Namespaces { get; init; }
    // ← No setters; once created, cannot be modified
}

// Created once at startup; shared across all threads:
var settings = new CommanderSettingsBuilder() /* ... */ .Build();

// Multiple threads read simultaneously:
for (int i = 0; i < 100; i++)
{
    Task.Run(() => {
        var conns = settings.Connections;  // Safe read
        var namespaces = settings.Namespaces;  // Safe read
    });
}

// ✅ 100% safe. No mutations; all reads are safe.
```

---

## Per-Request/Per-Thread Components (Not Shared)

### IDbConnection

Each request/thread gets its own connection:

```csharp
// DO IT:
var conn1 = connector.CreateConnection(setting);  // Thread 1
var conn2 = connector.CreateConnection(setting);  // Thread 2
// Two different connections; never shared

// ✗ DON'T DO:
IDbConnection shared = connector.CreateConnection(setting);
// Thread 1 uses shared
// Thread 2 uses shared  ← Data race!
```

### DbTransaction

Each connection has one active transaction:

```csharp
// Each Execute command gets its own transaction:
await _commander.ExecuteAsync(user1);  // Thread 1: new transaction
await _commander.ExecuteAsync(user2);  // Thread 2: new transaction
// Each transaction on separate connection; no sharing
```

---

## Concurrency Model

### Request Handling

```
Request from Thread 1
    ├─ ICommander<T> (shared service instance)
  ├─ Retrieve connection from pool (unique connection)
  ├─ Resolve command (cached settings)
  ├─ Execute via Dapper on connection
  ├─ Return results
  └─ Return connection to pool

Request from Thread 2
    ├─ ICommander<T> (same service instance, safe)
    ├─ Retrieve different connection from pool
  ├─ Resolve command (same cached settings, safe)
  ├─ Execute via Dapper on connection
  ├─ Return results
  └─ Return connection to pool

[Both threads running concurrently]
├─ Shared state: ICommander<T> (ConcurrentDictionary-backed)
└─ Private state: Connections, transactions
```

### High-Concurrency Example

```csharp
// ASP.NET Core handling 1000 concurrent HTTP requests:

var tasks = Enumerable.Range(0, 1000)
    .Select(async i =>
    {
        var user = await _userRepository.RetrieveAsync(i);
        await _userRepository.CreateAsync(new User { Name = $"User{i}" });
    });

await Task.WhenAll(tasks);

// At peak:
// ├─ 1000 threads running
// ├─ ~100 connections active (from pool, up to Max Pool Size)
// ├─ ICommander<T> singleton handling all
// ├─ Each thread Retrieves own connection
// └─ No data races; fully concurrent
```

---

## Builder Thread Safety

### CommanderSettingsBuilder

Thread-safe for construction:

```csharp
var builder = new CommanderSettingsBuilder();

// Multiple threads can call simultaneously:
Task.Run(() => builder.AddConnectionString("Conn1", "..."));
Task.Run(() => builder.AddConnectionString("Conn2", "..."));
Task.Run(() => builder.AddCommand(ns => { /* configure */ }));

// ✅ Safe. Builder uses ConcurrentDictionary internally.

// Build once. Then share immutable result:
var settings = builder.Build();
// From this point, settings is immutable and safe to share
```

---

## Guarantees

### ✅ Safe to Do

```csharp
// Share singleton across threads:
services.AddSingleton(typeof(ICommander<>), typeof(DatabaseCommander<>));
services.AddSingleton<IDatabaseConnector, DatabaseConnector>();

await Task.WhenAll(
    _userRepository.RetrieveAsync(1),
    _userRepository.RetrieveAsync(2),
    _userRepository.CreateAsync(new User { }),
    _userRepository.UpdateAsync(existingUser)
);  // ✅ 100% safe

// Share configuration across threads:
foreach (var thread in threads)
{
    thread.Settings = configuration;  // ✅ Safe
}
```

### ✗ Don't Do

```csharp
// Share connection across threads:
var conn = connector.CreateConnection(setting);
Task.Run(() => conn.ExecuteScalar(...));      // ✗ Not thread-safe
Task.Run(() => conn.ExecuteScalar(...));      // ✗ Data race

// Mutate configuration at runtime:
var settings = new CommanderSettings { ... };
// Can't mutate settings; it's sealed record

// Build while building:
var builder = new CommanderSettingsBuilder();
Task.Run(() => builder.AddCommand(...));      // OK, safe
var settings = builder.Build();
Task.Run(() => builder.AddCommand(...));      // ✗ Can't use after built
```

---

## Performance Implications

### Locking

All thread safety is achieved via `ConcurrentDictionary` (lock-free patterns):

```
No explicit locks used
├─ ConcurrentDictionary uses internal fine-grained locking
├─ Lock contention minimal (cache hits are read-only)
└─ Performance impact: typically low, but workload-dependent
```

### Contention Points

Potential contention only during:
1. **First command resolution**: LINQ query over settings (milliseconds)
2. **First connection lookup**: Settings search (microseconds)

After that: Contention-free reads from cache.

---

## Testing Thread Safety

For integration tests with high concurrency:

```csharp
[Fact]
public async Task ConcurrentRequestsAreThreadSafe()
{
    var tasks = Enumerable.Range(0, 100)
        .Select(i => _userRepository.RetrieveAsync(i))
        .Concat(Enumerable.Range(0, 50)
                .Select(i => _userRepository.CreateAsync(new User { /* ... */ })))
        .ToList();
    
    var results = await Task.WhenAll(tasks);  // ✅ Should complete without exceptions
}
```

---

## Related Pages

- [Architecture Overview](index.md)
- [Code Structure](../code-structure.md)
- [Performance Characteristics](../code-structure.md#performance-characteristics)



