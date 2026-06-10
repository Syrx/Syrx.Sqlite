# Connection Management

How Syrx.Commanders.Databases creates, caches, and manages database connections.

---

## Connection Lifecycle

```
Application Request
    ↓
DatabaseCommander requires connection
    ↓
IDatabaseConnector.CreateConnection(commandSetting)
    ├─ Resolve connection string:
    │  └─ commandSetting.ConnectionAlias → alias lookup cache
    │  └─ ConnectionStringSetting retrieved (cached after first lookup)
    ├─ Retrieve provider factory:
    │  └─ Via Func<DbProviderFactory> predicate
    │  └─ Allows SQL Server, PostgreSQL, MySQL, SQLite, etc.
    └─ Create connection:
       └─ factory.CreateConnection()
       └─ connection.ConnectionString = resolved string
       └─ Return IDbConnection
    ↓
[Execute query or command via Dapper]
    ↓
Connection returned to pool
    ├─ If in transaction: Transaction commits
    └─ Connection pooling handles reuse (ADO.NET responsibility)
```

---

## Connection Pooling

### ADO.NET Connection Pooling

Syrx uses ADO.NET's built-in connection pooling. You configure it via connection string parameters:

```csharp
// SQL Server connection string with pooling config
"Server=localhost;Database=MyDb;Integrated Security=true;
 Max Pool Size=100;
 Min Pool Size=5;
 Connection Lifetime=300;"

// PostgreSQL connection string
"Server=localhost;Database=MyDb;User Id=app;Password=...;
 Maximum Pool Size=100;
 Minimum Pool Size=5;
 Connection Idle Lifetime=300;"
```

### How Pooling Works

```
Connection Pool (per connection string)
├─ Created: When first connection requested
├─ Initial size: Min Pool Size
├─ Maximum size: Max Pool Size
├─ Lifetime: Connection Lifetime (seconds)
└─ Idle cleanup: Enabled by default

Request 1: Retrieve connection
├─ Check available pool
├─ Found: Return reusable connection
└─ Not found: Create new (up to max)

Request 2: Retrieve connection
├─ Check available pool
├─ Found: Return reusable (different connection, same pool)
└─ Reuse is faster than creating new

... (hundreds of reuses per connection before expiring)

Request N: Connection expires
├─ Connection Lifetime exceeded
└─ Connection replaced (removed, new one created)
```

### Performance Impact

- **First connection**: Higher cost due to pool initialization and connection establishment
- **Subsequent connections**: Lower cost when reused from pool
- **Overhead**: Minimal after warmup

---

## Connection Alias Caching

### Alias Resolution

Connection aliases are cached after first lookup:

```
DatabaseConnector._connectionCache
├─ Type: ConcurrentDictionary<string, ConnectionStringSetting>
├─ Key: Connection alias (e.g., "Default", "Reporting")
├─ Value: ConnectionStringSetting (alias + connection string)
└─ Lifetime: Application lifetime

First lookup of "Default":
├─ Check cache: Not found
├─ Read from settings: Search connections[]
├─ Cache: _connectionCache["Default"] = setting
└─ Cost: One-time settings lookup

Subsequent lookups of "Default":
├─ Check cache: Found
└─ Cost: Direct dictionary access
```

### Multiple Aliases

For applications using multiple databases:

```json
{
    "connections": [
        { "alias": "Default", "connectionString": "Server=primary;..." },
        { "alias": "Reporting", "connectionString": "Server=reporting;..." },
        { "alias": "Legacy", "connectionString": "Server=legacy;..." }
    ]
}
```

Each alias maintains its own connection pool.

---

## Thread Safety

### Connection Per Thread

```
Thread 1              Thread 2              Thread 3
  ↓                     ↓                     ↓
Retrieves conn from Retrieves conn from Retrieves conn from
primary pool        primary pool        primary pool
  ↓                     ↓                     ↓
[possibly different connection from same pool - that's OK]
  ↓                     ↓                     ↓
Executes query      Executes query      Executes query
  ↓                     ↓                     ↓
Returns to pool     Returns to pool     Returns to pool
```

**Policy**: Never hold a connection across requests. Always retrieve fresh from pool, use, return.

### ConcurrentDictionary Alias Cache

The connection alias cache is 100% thread-safe:

```csharp
_connectionCache = new ConcurrentDictionary<string, ConnectionStringSetting>();

// Multiple threads can call simultaneously:
Parallel.For(0, 1000, i =>
{
    var settings = _connectionCache.GetOrAdd("Default", 
        _ => LookupFromConfiguration("Default"));
    // Thread-safe. No race conditions.
    // First thread to call wins; others wait slightly. Result is the same.
});
```

---

## Configuration

### Connection String Formats

**SQL Server**:
```
Server=localhost;Database=MyDb;Integrated Security=true;
```

**PostgreSQL**:
```
Server=localhost;Database=MyDb;User Id=app;Password=${DB_PASSWORD};
```

**MySQL**:
```
Server=localhost;Database=MyDb;Uid=root;Pwd=${DB_PASSWORD};
```

**SQLite** (file-based):
```
Data Source=./MyDatabase.db;
```

### Pooling Settings

| Setting | Default | Impact |
|---------|---------|--------|
| `Max Pool Size` | 100 | Maximum concurrent connections |
| `Min Pool Size` | 5 | Connections kept ready |
| `Connection Lifetime` | unlimited | Seconds before connection is recycled |
| `Connection Idle Lifetime` | 0 | Seconds idle before removal (varies by provider) |

---

## Limits & Constraints

### Maximum Connections

Limited by:
1. ADO.NET `Max Pool Size` per alias
2. Database server maximum connections
3. OS network limits

Typical setup:
```
ADO.NET Max Pool Size = 100 per alias
Database Max Connections = 1000
Application server connections = ADO.NET total
```

### Connection Exhaustion

If all connections in pool are in use:

```
New request arrives
├─ Check pool: All in use
├─ Create new?: May exceed Max Pool Size
├─ Wait?: Pool waits for available connection
└─ Timeout?:  Wait timeout configurable per provider
```

Prevention:
- Monitor pool usage
- Set appropriate `Max Pool Size`
- Ensure queries complete quickly

### Long-Running Queries

```
Long-running query
├─ Holds connection from pool
├─ Other requests may wait for available connection
└─ Potential: Connection pool starvation

Solution:
├─ Optimize query performance
├─ Use `CommandTimeout` setting in configuration
├─ Break work into smaller queries if possible
```

---

## Provider Factory Pattern

Syrx supports any ADO.NET provider via `DbProviderFactory`:

```csharp
// SQL Server
builder.Services.UseDatabaseConnector(
    () => System.Data.SqlClient.SqlClientFactory.Instance);

// PostgreSQL (Npgsql)
builder.Services.UseDatabaseConnector(
    () => Npgsql.NpgsqlFactory.Instance);

// MySQL
builder.Services.UseDatabaseConnector(
    () => MySql.Data.MySqlClient.MySqlClientFactory.Instance);

// SQLite
builder.Services.UseDatabaseConnector(
    () => System.Data.SQLite.SQLiteFactory.Instance);

// Custom provider (if needed):
builder.Services.UseDatabaseConnector(
    () => new CustomDbProviderFactory());
```

---

## Best Practices

1. **Define connection aliases once at startup**: Don't create new aliases at runtime
2. **Use separate aliases for different databases**: Each gets own pool
3. **Monitor connection pool** (via provider-specific tools)
4. **Set appropriate pool sizes**: Balance responsibility between load and resources
5. **Close connections promptly**: ADO.NET connection pooling is transparent but depends on returning connections quickly
6. **Use async methods**: Reduces connection hold times

---

## Related Pages

- [Connection Management](../architecture/connection-management.md)
- [Transaction Handling](../architecture/transaction-handling.md)
- [Configuration Overview](../configuration/index.md)



