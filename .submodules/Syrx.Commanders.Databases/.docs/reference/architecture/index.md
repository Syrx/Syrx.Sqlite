# Architecture & Design Principles

Comprehensive overview of Syrx.Commanders.Databases core architecture, design patterns, and constraints.

---

## Core Design Philosophy

### Principles

1. **Configuration-Driven**: SQL commands live in configuration, not code
2. **Type-Safe**: Compile-time validation via `[CallerMemberName]`
3. **Least Privilege**: Repositories see only `ICommander<T>`
4. **Performance-First**: Caching at three levels (commands, connections, queries)
5. **Thread-Safe**: All shared state protected by concurrent data structures
6. **Async-First**: Async operations preferred for scalability
7. **Provider-Agnostic**: Supports any ADO.NET provider via factory pattern

### Design Trade-offs

| Choice | Benefit | Trade-off |
|--------|---------|-----------|
| Configuration external to code | Deployment-safe SQL changes | One more file to manage |
| Method name resolution | No annotations needed | Names must match exactly |
| Immutable settings | Thread-safe by default | Can't hot-reload configuration |
| ConcurrentDictionary caching | Thread-safe without locks | Slight memory overhead |
| `[CallerMemberName]` only | Zero reflection overhead | Only works for direct method calls |

---

## Layered Boundaries

### Boundaries & Contracts

```
┌──────────────────────────────────────────────────────┐
│ Public API Boundary                                  │
│ (What repositories see)                              │
├──────────────────────────────────────────────────────┤
│ ICommander<TRepository>                              │
│ - Query<TResult>(object? params, string? method)     │
│ - QueryAsync<TResult>(...)                           │
│ - Execute<TResult>(TResult model, ...)               │
│ - ExecuteAsync<TResult>(...)                         │
│ + Multi-mapping variants (T1-T16)                    │
│ + Multiple result set variants                       │
└──────────────────────────────────────────────────────┘
       │ Implemented by
       ▼
┌──────────────────────────────────────────────────────┐
│ Implementation Boundary                              │
│ (Framework only - not public)                        │
├──────────────────────────────────────────────────────┤
│ DatabaseCommander<TRepository> (internal impl)       │
│ - Resolves method name → CommandSetting              │
│ - Manages lifetime (Dispose)                         │
│ - Per-operation caching                              │
└──────────────────────────────────────────────────────┘
       │ Depends on
       ▼
┌──────────────────────────────────────────────────────┐
│ Configuration Boundary                               │
│ (Immutable settings model)                           │
├──────────────────────────────────────────────────────┤
│ ICommanderSettings (root contract)                   │
│ - NamespaceSetting[]                                 │
│ - TypeSetting[]                                      │
│ - CommandSetting[]                                   │
│ - ConnectionStringSetting[]                          │
│ (All sealed records - immutable after construction)  │
└──────────────────────────────────────────────────────┘
       │ Loaded from
       ▼
┌──────────────────────────────────────────────────────┐
│ Data Access Boundary                                 │
│ (Abstractions for connection & command resolution)   │
├──────────────────────────────────────────────────────┤
│ IDatabaseConnector                                   │
│ - CreateConnection(CommandSetting): IDbConnection   │
│                                                      │
│ IDatabaseCommandReader                               │
│ - GetCommand(Type, string): CommandSetting          │
└──────────────────────────────────────────────────────┘
```

---

## Architectural Patterns

### 1. Repository Pattern

**How It Works:**

```csharp
// Application code doesn't instantiate commanders
// Instead: inject ICommander<T>

public class UserRepository
{
    private readonly ICommander<UserRepository> _commander;
    
    public UserRepository(ICommander<UserRepository> commander)
    {
        _commander = commander;  // Framework-managed service instance
    }
    
    public async Task<User?> RetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        return (await _commander.QueryAsync<User>(new { id }, cancellationToken)).FirstOrDefault();
    }

    public async Task<IEnumerable<User>> RetrieveAllAsync(
        int page = 1,
        int size = 100,
        CancellationToken cancellationToken = default)
    {
        return await _commander.QueryAsync<User>(
            new { offset = (page - 1) * size, size },
            cancellationToken);
    }
}
```

**Benefits:**
- Repositories stay focused on business logic
- SQL managed separately from code
- Easy testing via `ICommander<T>` mocks
- Multiple query methods map to different SQL commands

---

### 2. Configuration-Driven Resolution

**How It Works:**

```
Repository method call:
    UserRepository.RetrieveAsync(id: 42)
        ↓
[CallerMemberName] captures: "RetrieveAsync"
        ↓
DatabaseCommander combines:
    - method name → "RetrieveAsync"
    - typeof(TRepository).FullName → "MyApp.Data.UserRepository"
    - Default namespace: "MyApp.Data"
        ↓
Command key formed:
    "MyApp.Data.UserRepository.RetrieveAsync"
        ↓
Lookup in configuration:
    namespaces[0].namespace = "MyApp.Data"
        types[0].name = "UserRepository"
             commands["RetrieveAsync"] = {
                 "commandText": "SELECT ...",
                 "connectionAlias": "Default"
             }
        ↓
CommandSetting resolved, SQL executed
```

**Why This Approach?**

- **No annotations needed**: Method names are enough
- **No reflection**: [CallerMemberName] is compile-time
- **Centralized management**: All SQL in one place
- **Type-safe**: Mismatches caught at integration test time

---

### 3. Builder Pattern

For programmatic configuration assembly:

```csharp
// Builder is thread-safe (uses ConcurrentDictionary internally)

var settings = new CommanderSettingsBuilder()
    .AddConnectionString("Default", "Server=localhost;...")
    .AddConnectionString("Reporting", "Server=reports;...")
    .AddCommand(ns => ns
        .UseNamespace("MyApp.Data")
        .AddType(type => type
            .UseName("UserRepository")
            .AddCommand("RetrieveAsync", cmd => cmd
                .UseCommandText("SELECT * FROM Users WHERE Id = @id")
                .UseConnectionAlias("Default")
                .UseCommandTimeout(30)
            )
        )
    )
    .Build();
```

**Benefits:**
- Fluent API for readability
- No JSON/XML files needed if preferred
- Type checking at build time
- Composable (combine multiple builders)

---

### 4. Partial Classes

`DatabaseCommander<T>` is organized across 7 files by concern:

```
DatabaseCommander.cs                  # Core class, constructor, disposal
DatabaseCommander.Execute.cs          # Sync execute operations
DatabaseCommander.ExecuteAsync.cs     # Async execute operations
DatabaseCommander.Query.*.cs          # Query operations (sync & async)
DatabaseCommander.Query.Multimap.cs   # Multi-mapping support
DatabaseCommander.QueryAsync.*.cs     # Async variants
```

**Benefits:**
- Each method group in its own file
- Easier to navigate and maintain
- Compiler combines into one class at runtime

---

### 5. Adapter Pattern

`IDatabaseConnector` abstracts provider-specific connection creation:

```csharp
// Application code never directly creates connections

public interface IDatabaseConnector
{
    IDbConnection CreateConnection(CommandSetting setting);
}

public class DatabaseConnector : IDatabaseConnector
{
    private readonly Func<DbProviderFactory> _providerPredicate;
    
    public IDbConnection CreateConnection(CommandSetting setting)
    {
        var factory = _providerPredicate();  // SQL Server, PG, MySQL, etc.
        var connection = factory.CreateConnection();
        connection.ConnectionString = /* resolved from setting */;
        return connection;
    }
}
```

**Enables:**
- Swapping providers (SQL Server ↔ PostgreSQL) via single factory
- Custom connector implementations for special scenarios
- Provider-agnostic business logic

---

### 6. Dependency Injection Layering

DI is stratified by layer:

```csharp
// Level 4: Root composition (user calls this)
builder.Services.UseSyrx(config => 
    config.UseFile("syrx.json", settings => { })
);

    ↓ Uses internally
    ├─ Level 3: Core registration
    ├─ Level 2: Connector + Reader registration
    └─ Level 1: Individual component registration
```

Each level has a clear Single Responsibility.

---

## Critical Architecture Constraints

### 1. Configuration is Immutable After Startup

```csharp
// Configuration is a sealed record (immutable by default)
public sealed record CommanderSettings : ICommanderSettings
{
    public required List<ConnectionStringSetting> Connections { get; init; }
    public required List<NamespaceSetting> Namespaces { get; init; }
}

// Cannot be changed after construction
// Shared safely across all threads
```

**Consequence**: No runtime configuration changes. Plan around this.

### 2. Method Names Must Match Exactly

Configuration lookup is **case-sensitive and exact-match**:

```csharp
// Code:
public async Task RetrieveAsync(int id, CancellationToken cancellationToken = default) { ... }

// Must have exact match in config (including casing):
"commands": {
    "RetrieveAsync": {  // ← Must be identical
        "commandText": "..."
    }
}

// These won't match:
// "retrieveasync"          ✗ Wrong case
// "RetrieveAllAsync"       ✗ Different method name
// "RetrieveAsync "         ✗ Extra space
```

### 3. [CallerMemberName] Only Works for Direct Calls

Command resolution uses `[CallerMemberName]`. This works:

```csharp
public async Task<User?> RetrieveAsync(int id, CancellationToken cancellationToken = default)
{
    return (await _commander.QueryAsync<User>(new { id }, cancellationToken)).FirstOrDefault();
}  // ✅ QueryAsync knows caller is "RetrieveAsync"
```

This **doesn't work**:

```csharp
private async Task<IEnumerable<User>> QueryUsers(CancellationToken cancellationToken = default)
{
    return await _commander.QueryAsync<User>(cancellationToken: cancellationToken);  // ✗ Caller name is "QueryUsers", not your repository method
}

public async Task<User?> RetrieveAsync(int id, CancellationToken cancellationToken = default)
{
    return (await QueryUsers(cancellationToken)).FirstOrDefault();  // Won't resolve correctly
}
```

**Solutions**: Call `_commander` directly from public repository methods. Prefer distinct names such as `RetrieveAllAsync` for collection queries; if overloads are unavoidable, pass `method:` explicitly on the direct `_commander` call.

### 4. Connection Pooling Requires Consistent Aliases

Connection strings are cached by alias. Changing an alias at runtime won't affect running code:

```csharp
// BAD - won't work:
_connector.CreateConnection(new CommandSetting { ConnectionAlias = "NewAlias" });
// "NewAlias" not in settings; lookup fails

// GOOD - predefined in configuration:
{
    "connections": [
        { "alias": "Default", "connectionString": "..." },
        { "alias": "Reporting", "connectionString": "..." }
    ]
}
```

### 5. Transactions Are Per-Execute Only

`Execute` operations wrap in transactions. `Query` operations **do not**:

```csharp
await _commander.QueryAsync<User>();  // No transaction

await _commander.ExecuteAsync(user);   // Wrapped in transaction
                                        // Auto-commit on success
                                        // Auto-rollback on exception
```

If you need queries in a transaction, manage manually via Dapper.

---

## Thread Safety Guarantee

### Safe to Share Across Threads

✅ These components are thread-safe and designed for concurrent use:

- `DatabaseCommander<TRepository>` (safe in transient, scoped, or singleton lifetimes)
- `DatabaseConnector` (safe in transient, scoped, or singleton lifetimes)
- `DatabaseCommandReader` (safe in transient, scoped, or singleton lifetimes)
- All configuration records (sealed, immutable)

```csharp
// This is safe:
var commander = serviceProvider.GetRequiredService<ICommander<UserRepository>>();

// Multiple threads can call simultaneously:
Task.WhenAll(
    commander.QueryAsync<User>(),
    commander.QueryAsync<User>(),
    commander.QueryAsync<User>()
);  // ✅ Safe. Each thread Retrieves own connection from pool.
     // Shared caches (commands, aliases) are ConcurrentDictionary-backed.
```

### Not Thread-Safe (By Design)

⚠️ Do not share these across threads without synchronization:

- Individual `IDbConnection` instances (use connection pooling)
- Active `DbTransaction` instances (per-connection, per-thread)
- Builder instances during construction (build once, then share immutable result)

---

## Performance Guarantees

### Caching Guarantees

1. **Command Cache**: 
   - Lookup cached after first resolution
   - Cache key: `"Namespace.Type.Method"`
    - Hit rate: typically high after warmup (workload-dependent)

2. **Connection Alias Cache**:
   - Lookup cached after first resolution
   - Cache key: Connection alias string
   - Hit rate: 100% after first request

3. **Dapper Compiled Query Cache**:
    - Dapper supports command/query plan caching
    - Effective reuse depends on command flags and execution settings
    - Review `CommandSetting.Flags` defaults when tuning cache behavior

### Allocation Guarantees

Per-request allocations (typical query):
- 1 `IDbConnection` (from pool, not allocated)
- 1 `DbCommand` (ADO.NET overhead)
- 1 `CommandDefinition` (Dapper DTO)
- Result collection (proportional to data)

Hot-path notes:
- Command and alias lookups are cache-backed and efficient
- Runtime still performs normal per-call work (for example, cache key construction)
- Avoid assuming zero-allocation behavior without measurement

---

## Constraints Summary

| Constraint | Mitigation |
|-----------|----------|
| Configuration immutable | Load once at startup; freeze for application lifetime |
| Method names must match exactly | Careful naming discipline; lint/test to verify |
| [CallerMemberName] direct-only | Always call `_commander` from public repository methods |
| No runtime config changes | Plan configuration before startup |
| Connection pools by alias only | Define all aliases needed upfront |
| Execute-only transactions | Manage query transactions manually if needed |

---

## Related Pages

- [Command Resolution](command-resolution.md) — Deep dive into method → SQL mapping
- [Connection Management](connection-management.md) — Connection lifecycle and pooling
- [Transaction Handling](transaction-handling.md) — Transaction behavior and failure modes
- [Thread Safety](thread-safety.md) — Detailed concurrency model
- [Code Structure](../code-structure.md) — Visual architecture diagram



