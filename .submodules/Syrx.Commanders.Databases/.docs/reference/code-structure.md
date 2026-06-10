# Code Structure & Architecture

Visual overview of the project architecture, layering, and dependency relationships.

---

## Project Organization

### All 11 Projects at a Glance

```
Syrx.Commanders.Databases.Solution
│
├── 🔴 CORE (Execution Engine)
│   └── Syrx.Commanders.Databases
│       └── DatabaseCommander<TRepository> (main public API)
│
├── 🟡 DATA ACCESS (Connection Abstraction)
│   ├── Syrx.Commanders.Databases.Connectors
│   │   └── IDatabaseConnector / DatabaseConnector
│   └── Syrx.Commanders.Databases.Connectors.Extensions
│       └── ServiceCollectionExtensions (DI registration)
│
├── 🔵 CONFIGURATION (Settings & Resolution)
│   ├── Syrx.Commanders.Databases.Settings
│   │   └── CommanderSettings, NamespaceSetting, TypeSetting, CommandSetting
│   ├── Syrx.Commanders.Databases.Settings.Extensions
│   │   └── CommanderSettingsBuilder, fluent API
│   ├── Syrx.Commanders.Databases.Settings.Extensions.Json
│   │   └── JSON file loader
│   ├── Syrx.Commanders.Databases.Settings.Extensions.Xml
│   │   └── XML file loader
│   ├── Syrx.Commanders.Databases.Settings.Readers
│   │   └── IDatabaseCommandReader / DatabaseCommandReader
│   └── Syrx.Commanders.Databases.Settings.Readers.Extensions
│       └── ServiceCollectionExtensions (reader DI)
│
├── 🟢 UTILITIES (Schema Modeling)
│   └── Syrx.Commanders.Databases.Builders
│       └── Database, Table, Field classes (DDL scenarios)
│
└── ⚪ DI EXTENSIONS (Root Setup)
    └── Syrx.Commanders.Databases.Extensions
        └── ServiceCollectionExtensions (UseSyrx root registration)
```

---

## Layered Architecture

### Permission Hierarchy

```
┌─────────────────────────────────────────────────┐
│         Application Layer                       │
│   (Your Repositories, Services, Controllers)    │
│                                                 │
│  ├─ Dependency: ICommander<TRepository>         │
│  └─ Permission: Query, Execute, multi-mapping   │
└──────────────────┬──────────────────────────────┘
                   │ (Injected)
┌──────────────────▼──────────────────────────────┐
│       DatabaseCommander<TRepository>            │  [CORE]
│    (Command Execution Engine)                   │
│                                                 │
│  ├─ Resolves: method name → SQL command         │
│  ├─ Manages: connections and transactions       │
│  └─ Executes: via Dapper ORM                    │
│  ├─ Caches: commands (ConcurrentDictionary)     │
│  └─ Thread-Safe: Yes                            │
└──────────────────┬──────────────────────────────┘
                   │
      ┌────────────┼────────────┐
      │            │            │
┌─────▼────┐  ┌────▼─────┐ ┌────▼─────────┐
│ Command  │  │ Database │ │ Configuration│
│ Resolver │  │Connector │ │  Reader      │
└──────────┘  └──────────┘ └──────────────┘
      │            │            │
      │            │            │
┌─────▼────────────▼────────────▼──────────┐
│     ICommanderSettings (Configuration)   │  [CONFIG]
│                                          │
│  • Connections (aliases & strings)       │
│  • Namespaces ├─ Types ├─ Commands       │
│  • All settings are immutable records    │
└──────────────────┬───────────────────────┘
                   │
┌──────────────────▼──────────────────────┐
│     Configuration Sources               │  [LOADERS]
│                                         │
│  • JSON files                           │
│  • XML files                            │
│  • Builder API (programmatic)           │
│  • Environment variables (via builder)  │
└──────────────────┬──────────────────────┘
                   │
┌──────────────────▼──────────────────────┐
│    Database Connections & Providers     │  [DATA ACCESS]
│                                         │
│  • DbProviderFactory (SQL Server, PG...)│
│  • Connection pooling (ADO.NET)         │
│  • Dapper ORM integration               │
└──────────────────┬──────────────────────┘
                   │
                   ▼
              SQL Database
```

---

## Component Interactions

### Normal Query Execution Flow

```
1. Repository Method Call
   └─ UserRepository.RetrieveAsync(id: 42)

2. ICommander<TRepository> Resolves Method
   └─ Namespace: MyApp.Data
   └─ Type: UserRepository
    └─ Method: RetrieveAsync
   └─ Uses [CallerMemberName] (no reflection!)

3. Command Lookup (Cached)
    └─ Check: _commandCache["MyApp.Data.UserRepository.RetrieveAsync"]
   └─ If miss: Load from IDatabaseCommandReader
   └─ Result: CommandSetting { CommandText: "SELECT...", ... }

4. Retrieve Database Connection (Cached)
   └─ IDatabaseConnector.CreateConnection(commandSetting)
   └─ Resolve: commandSetting.ConnectionAlias → connection string
   └─ Create: IDbConnection via provider factory
   └─ Pool: ADO.NET connection pooling handles lifecycle

5. Execute via Dapper
   └─ Build: CommandDefinition with settings
   └─ Call: connection.QueryAsync<T>(commandDef)
   └─ Map: Results to TResult via Dapper

6. Return to Application
   └─ IEnumerable<TResult> returned to repository
```

### Execute Command Flow

```
1. Repository Execute Call
    └─ UserRepository.CreateAsync(user)

2. Command Resolution (Same as Query)
   └─ Look up: namespace.type.method → CommandSetting

3. Transaction Wrapper
   └─ Create: DbTransaction (IsolationLevel from config)
   └─ Connection: Obtain from pool

4. Execute via Dapper
   └─ Build: CommandDefinition with settings
   └─ Call: connection.ExecuteAsync(commandDef, transaction)
   └─ Result: Affected row count

5. Commit/Rollback
   └─ Success: transaction.Commit()
   └─ Exception: transaction.Rollback() (exceptions bubble)

6. Return Success/Failure
   └─ bool: (affected > 0) returned to repository
```

---

## Dependency Graph

### Direct Dependencies Between Projects

```
Syrx.Commanders.Databases
    ├─ Syrx.Commanders.Databases.Connectors
    ├─ Syrx.Commanders.Databases.Settings
    ├─ Syrx.Commanders.Databases.Settings.Readers
    └─ Dapper, Syrx (submodule)

Syrx.Commanders.Databases.Connectors
    ├─ Syrx.Commanders.Databases.Settings
    └─ System.Data.Common

Syrx.Commanders.Databases.Settings
    └─ Syrx (submodule) [for ICommandSetting interface]

Syrx.Commanders.Databases.Settings.Extensions
    ├─ Syrx.Commanders.Databases.Settings
    └─ Syrx.Commanders.Databases.Settings.Readers

Syrx.Commanders.Databases.Settings.Extensions.Json
    ├─ Syrx.Commanders.Databases.Settings
    ├─ Syrx.Commanders.Databases.Settings.Extensions
    └─ System.Text.Json

Syrx.Commanders.Databases.Settings.Extensions.Xml
    ├─ Syrx.Commanders.Databases.Settings
    ├─ Syrx.Commanders.Databases.Settings.Extensions
    └─ System.Xml.Linq

Syrx.Commanders.Databases.Settings.Readers
    ├─ Syrx.Commanders.Databases.Settings
    └─ Syrx (submodule) [for ICommandReader interface]

Syrx.Commanders.Databases.Builders
    └─ System.Data (SqlDbType enum)

Syrx.Commanders.Databases.Connectors.Extensions
    ├─ Syrx.Commanders.Databases.Connectors
    └─ Microsoft.Extensions.DependencyInjection

Syrx.Commanders.Databases.Settings.Readers.Extensions
    ├─ Syrx.Commanders.Databases.Settings.Readers
    └─ Microsoft.Extensions.DependencyInjection

Syrx.Commanders.Databases.Extensions
    ├─ Syrx.Commanders.Databases
    ├─ Syrx.Commanders.Databases.Settings.Extensions
    ├─ Syrx.Commanders.Databases.Connectors.Extensions
    ├─ Syrx.Commanders.Databases.Settings.Readers.Extensions
    └─ Microsoft.Extensions.DependencyInjection
```

### Dependency Levels

```
Level 0 (Foundation, No Framework Dependencies)
    ├─ Builders (Database, Table, Field)
    └─ Settings (records + configuration models)

Level 1 (Infrastructure Abstractions)
    ├─ Connectors (IDatabaseConnector)
    └─ Settings.Readers (IDatabaseCommandReader)

Level 2 (Core Implementation)
    └─ Syrx.Commanders.Databases (DatabaseCommander<T>)

Level 3 (Extended Services)
    ├─ Settings.Extensions (Builder API)
    ├─ Settings.Extensions.Json (JSON loader)
    ├─ Settings.Extensions.Xml (XML loader)
    ├─ Connectors.Extensions (DI setup)
    └─ Settings.Readers.Extensions (DI setup)

Level 4 (Root Composition)
    └─ Extensions (UseSyrx root entry point)
```

---

## Public API Summary

### By Project

| Project | Public Types | Interfaces | Classes | Records | Enums |
|---------|--------------|-----------|---------|---------|-------|
| Commanders.Databases | 1 | 0 | 1 | 0 | 0 |
| Builders | 6 | 0 | 3 | 3 | 0 |
| Connectors | 2 | 1 | 1 | 0 | 0 |
| Connectors.Extensions | 2 | 0 | 2 | 0 | 0 |
| Extensions | 1 | 0 | 1 | 0 | 0 |
| Settings | 6 | 1 | 0 | 5 | 1 |
| Settings.Extensions | 5 | 0 | 5 | 0 | 0 |
| Settings.Extensions.Json | 2 | 0 | 2 | 0 | 0 |
| Settings.Extensions.Xml | 2 | 0 | 2 | 0 | 0 |
| Settings.Readers | 1 | 1 | 0 | 0 | 0 |
| Settings.Readers.Extensions | 1 | 0 | 1 | 0 | 0 |
| **TOTAL** | **29** | **3** | **18** | **8** | **1** |

---

## Concurrency & Thread Safety

### Thread-Safe Components

✅ **Fully Thread-Safe** (all concurrent operations use `ConcurrentDictionary`):

- `DatabaseCommander<TRepository>` — Command cache
- `DatabaseConnector` — Connection alias cache
- `DatabaseCommandReader` — Command resolution cache
- All builder classes — ConcurrentDictionary backing

#### Thread-Safe Guarantees

```
┌──────────────────────────────────────────┐
│ Multiple Threads Calling Repository      │
├──────────────────────────────────────────┤
│  Thread 1  Thread 2  Thread 3            │
├──────────────────────────────────────────┤
│      ↓         ↓         ↓               │
│   ICommander<T> (shared instance)        │
├──────────────────────────────────────────┤
│                                          │
│  Command Cache (ConcurrentDict)          │
│  ├─ GetOrAdd() → thread-safe             │
│  └─ No duplicate lookups                 │
│                                          │
│  Connection Pool (ADO.NET)               │
│  ├─ Pool thread-safe                     │
│  └─ Connections returned to pool         │
│                                          │
│  Each Thread:                            │
│  ├─ Retrieves own connection from pool   │
│  └─ Retrieves shared cached command      │
└──────────────────────────────────────────┘
```

### Non-Thread-Safe (By Design)

Configuration objects are **immutable after construction** and safe to share:
- `CommanderSettings` (sealed record)
- `NamespaceSetting` (sealed record)
- `TypeSetting` (sealed record)
- `CommandSetting` (sealed record)

**Policy**: Build configuration once at startup; freeze and share.

---

## Performance Characteristics

### Caching Strategy

```
Level 1: Command Resolution Cache
├─ Key: "Namespace.Type.Method"
├─ Value: CommandSetting
├─ Location: DatabaseCommander._commandCache
├─ Lifetime: Application (cleared on restart)
├─ Hit Rate: typically high after warmup (workload-dependent)
└─ Cost of Miss: Single LINQ query over settings

Level 2: Connection Alias Cache
├─ Key: Connection alias string
├─ Value: ConnectionStringSetting
├─ Location: DatabaseConnector._connectionCache
├─ Lifetime: Application
├─ Hit Rate: 100% (after first use)
└─ Cost of Miss: Single collection lookup

Level 3: Dapper Compiled Query Cache
├─ Handled by: Dapper internally
├─ Behavior depends on command flags and execution settings
└─ Validate benefits under your runtime configuration
```

### Allocation Profile

- **Per Request** (typical query):
  - 1 connection (from pool, not allocated)
  - 1 DbCommand (allocation at native level)
  - Result collection (proportional to result set size)
  
- **Low-allocation lookups**:
    - Command lookup (cache-backed)
    - Caching operations (ConcurrentDictionary-backed)
    - Connection alias resolution (cache-backed)

---

## Next Steps

Deep dives:
- [Command Resolution](architecture/command-resolution.md) — How method names map to SQL commands
- [Connection Management](architecture/connection-management.md) — Connection pooling and lifetimes
- [Transaction Handling](architecture/transaction-handling.md) — Transaction semantics
- [Thread Safety](architecture/thread-safety.md) — Concurrency guarantees
- [Project Reference](projects/index.md) — API details for each project



