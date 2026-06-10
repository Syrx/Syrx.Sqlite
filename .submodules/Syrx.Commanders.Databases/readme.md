# Syrx.Commanders.Databases Documentation

> **Database command execution framework for .NET applications**

The Syrx.Commanders.Databases ecosystem provides a comprehensive, high-performance database access framework built on top of Dapper. It enables type-safe, configuration-driven database operations with support for multiple database providers, connection management, and command resolution.

## Table of Contents

- [Architecture Overview](#architecture-overview)
- [Core Concepts](#core-concepts)
- [Package Ecosystem](#package-ecosystem)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Advanced Topics](#advanced-topics)
- [Performance Considerations](#performance-considerations)
- [Migration Guide](#migration-guide)
- [API Reference](#api-reference)
- [Examples](#examples)

## Architecture Overview

The Syrx framework follows a layered architecture that separates concerns between application code, command resolution, connection management, and database execution:

```
┌─────────────────────────────────────────────────────────────┐
│                    Application Layer                        │
│  ┌─────────────────┐    ┌─────────────────┐                 │
│  │   Repository    │    │   Repository    │   ...           │
│  │     Class       │    │     Class       │                 │
│  └─────────────────┘    └─────────────────┘                 │
└─────────────────┬───────────────┬───────────────────────────┘
                  │               │
                  ▼               ▼
┌─────────────────────────────────────────────────────────────┐
│                 Commander Layer                             │
│  ┌─────────────────────────────────────────────────────────┐│
│  │           ICommander<TRepository>                       ││
│  │                                                         ││
│  │  ┌─────────────────────────────────────────────────────┐││
│  │  │       DatabaseCommander<TRepository>                │││
│  │  └─────────────────────────────────────────────────────┘││
│  └─────────────────────────────────────────────────────────┘│
└─────────────────┬───────────────┬───────────────────────────┘
                  │               │
                  ▼               ▼
┌─────────────────────────────────────────────────────────────┐
│              Configuration Layer                            │
│  ┌──────────────────┐    ┌──────────────────┐               │
│  │ Command Settings │    │ Connection       │               │
│  │    Resolution    │    │   Management     │               │
│  └──────────────────┘    └──────────────────┘               │
└─────────────────┬───────────────┬───────────────────────────┘
                  │               │
                  ▼               ▼
┌─────────────────────────────────────────────────────────────┐
│                Database Layer                               │
│  ┌──────────────────┐    ┌──────────────────┐               │
│  │     Dapper       │    │  ADO.NET         │               │
│  │   (Micro-ORM)    │    │ Connections      │               │
│  └──────────────────┘    └──────────────────┘               │
└─────────────────────────────────────────────────────────────┘
```

### Key Architectural Principles

1. **Separation of Concerns**: Each layer has distinct responsibilities
2. **Configuration-Driven**: SQL commands are externalized from code
3. **Type Safety**: Strong typing throughout the execution pipeline
4. **Performance First**: Built on Dapper for optimal database performance
5. **Provider Agnostic**: Support for multiple database providers
6. **Dependency Injection**: First-class DI container support

## Core Concepts

### Repository Pattern
Repositories define business operations that translate to database commands:

```csharp
public class UserRepository
{
    private readonly ICommander<UserRepository> _commander;
    
    public UserRepository(ICommander<UserRepository> commander)
    {
        _commander = commander;
    }
    
    // Method name automatically maps to configured command
    public async Task<User> RetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
      var result = await _commander.QueryAsync<User>(new { id }, cancellationToken);
        return result.FirstOrDefault();
    }
}
```

### Command Resolution
Commands are resolved using the pattern: `{Namespace}.{ClassName}.{MethodName}`

For the example above:
- **Namespace**: `MyApp.Repositories`
- **Class**: `UserRepository` 
- **Method**: `RetrieveAsync`
- **Resolved Command**: `MyApp.Repositories.UserRepository.RetrieveAsync`

### Connection Management
Named connection strings are resolved by alias, enabling environment-specific configuration:

```json
{
  "Connections": [
    {
      "Alias": "Primary",
      "ConnectionString": "Server=prod-db;Database=MyApp;..."
    },
    {
      "Alias": "ReadOnly",
      "ConnectionString": "Server=readonly-db;Database=MyApp;..."
    }
  ]
}
```

### Transaction Management
- **Query Operations**: Execute without transactions (read-only)
- **Execute Operations**: Automatically wrapped in transactions with rollback on failure

## Package Ecosystem

The Syrx.Commanders.Databases ecosystem consists of several interconnected packages:

### Core Packages

| Package | Purpose | Dependencies |
|---------|---------|--------------|
| **[Syrx.Commanders.Databases](../src/Syrx.Commanders.Databases/README.md)** | Core command execution engine | Syrx, Dapper |
| **[Syrx.Commanders.Databases.Connectors](../src/Syrx.Commanders.Databases.Connectors/README.md)** | Database connection abstractions | Syrx.Connectors |
| **[Syrx.Commanders.Databases.Settings](../src/Syrx.Commanders.Databases.Settings/README.md)** | Configuration model definitions | Syrx.Settings |

### Extension Packages

| Package | Purpose | Use Case |
|---------|---------|-----------|
| **[Syrx.Commanders.Databases.Extensions](../src/Syrx.Commanders.Databases.Extensions/README.md)** | DI container registration | Service registration |
| **[Syrx.Commanders.Databases.Connectors.Extensions](../src/Syrx.Commanders.Databases.Connectors.Extensions/README.md)** | Connector DI extensions | Connector registration |

### Configuration Packages

| Package | Purpose | Configuration Format |
|---------|---------|---------------------|
| **[Syrx.Commanders.Databases.Settings.Extensions](../src/Syrx.Commanders.Databases.Settings.Extensions/README.md)** | Recommended builder pattern APIs | Programmatic |
| **[Syrx.Commanders.Databases.Settings.Extensions.Json](../src/Syrx.Commanders.Databases.Settings.Extensions.Json/README.md)** | Optional JSON configuration loader | JSON files |
| **[Syrx.Commanders.Databases.Settings.Extensions.Xml](../src/Syrx.Commanders.Databases.Settings.Extensions.Xml/README.md)** | Optional XML configuration loader | XML files |

### Utility Packages

| Package | Purpose | Use Case |
|---------|---------|-----------|
| **[Syrx.Commanders.Databases.Builders](../src/Syrx.Commanders.Databases.Builders/README.md)** | Schema modeling utilities | Code generation, tooling |
| **[Syrx.Commanders.Databases.Settings.Readers](../src/Syrx.Commanders.Databases.Settings.Readers/README.md)** | Internal command resolution | Framework internals |
| **[Syrx.Commanders.Databases.Settings.Readers.Extensions](../src/Syrx.Commanders.Databases.Settings.Readers.Extensions/README.md)** | Reader DI extensions | Internal DI registration |

## Getting Started

### 1. Installation

Install the core package and the recommended builder configuration package:

```bash
# Core framework
dotnet add package Syrx.Commanders.Databases

# Recommended: builder-based configuration
dotnet add package Syrx.Commanders.Databases.Settings.Extensions

# Optional: file-based configuration loaders
dotnet add package Syrx.Commanders.Databases.Settings.Extensions.Json
dotnet add package Syrx.Commanders.Databases.Settings.Extensions.Xml

# Service registration extensions
dotnet add package Syrx.Commanders.Databases.Extensions
```

### 2. Configuration

Create configuration with the fluent builder API:

```csharp
var settings = new CommanderSettingsBuilder()
  .AddConnectionString("DefaultConnection", connectionString)
  .AddNamespace("MyApp.Repositories", ns => ns
    .AddType("UserRepository", type => type
      .AddCommand("RetrieveAsync", cmd => cmd
        .UseCommandText("SELECT * FROM Users WHERE Id = @id")
        .UseConnectionAlias("DefaultConnection"))
      .AddCommand("CreateAsync", cmd => cmd
        .UseCommandText("INSERT INTO Users (Name, Email) VALUES (@Name, @Email)")
        .UseConnectionAlias("DefaultConnection"))))
  .Build();
```

Optional JSON file equivalent:

```json
{
  "Connections": [
    {
      "Alias": "DefaultConnection",
      "ConnectionString": "Server=localhost;Database=MyApp;Trusted_Connection=true;"
    }
  ],
  "Namespaces": [
    {
      "Name": "MyApp.Repositories",
      "Types": [
        {
          "Name": "UserRepository",
          "Commands": {
            "RetrieveAsync": {
              "CommandText": "SELECT * FROM Users WHERE Id = @id",
              "ConnectionAlias": "DefaultConnection"
            },
            "CreateAsync": {
              "CommandText": "INSERT INTO Users (Name, Email) VALUES (@Name, @Email)",
              "ConnectionAlias": "DefaultConnection"
            }
          }
        }
      ]
    }
  ]
}
```

### 3. Service Registration

Register services in your DI container:

```csharp
public void ConfigureServices(IServiceCollection services)
{
  // Recommended: register builder-composed settings
  services.AddSingleton<ICommanderSettings>(settings);
  services.UseSyrx(_ => { });

  // Optional file-based registration
  // var configBuilder = new ConfigurationBuilder();
  // services.UseSyrx(builder => builder.UseFile("syrx.json", configBuilder));
    
    // Register your repositories
    services.AddScoped<UserRepository>();
}
```

### 4. Repository Implementation

Create your repository classes:

```csharp
public class UserRepository
{
    private readonly ICommander<UserRepository> _commander;
    
    public UserRepository(ICommander<UserRepository> commander)
    {
        _commander = commander;
    }
    
    public async Task<User> RetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
      var result = await _commander.QueryAsync<User>(new { id }, cancellationToken);
        return result.FirstOrDefault();
    }
    
    public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
      return await _commander.ExecuteAsync(user, cancellationToken) ? user : null;
    }
}
```

## Configuration

### Configuration Hierarchy

Configuration follows a hierarchical structure:

```
CommanderSettings
├── Connections[]           # Named connection strings
│   ├── Alias              # Connection name/reference
│   └── ConnectionString   # Actual connection string
└── Namespaces[]           # Namespace-level grouping
    ├── Name               # Namespace name
    └── Types[]            # Type-level grouping
        ├── Name           # Class name
        └── Commands{}     # Method-to-command mappings
            └── [MethodName]
                ├── CommandText      # SQL command
                ├── ConnectionAlias  # Connection reference
                ├── CommandTimeout   # Timeout in seconds
                ├── CommandType      # Text/StoredProcedure/TableDirect
                ├── SplitOn         # Multi-mapping split columns
                └── IsolationLevel  # Transaction isolation
```

### Configuration Sources

#### JSON Configuration
```json
{
  "Connections": [...],
  "Namespaces": [...]
}
```

#### XML Configuration
```xml
<CommanderSettings>
  <Connections>...</Connections>
  <Namespaces>...</Namespaces>
</CommanderSettings>
```

#### Programmatic Configuration
```csharp
var settings = new CommanderSettingsBuilder()
    .AddConnectionString("Default", connectionString)
    .AddNamespace("MyApp.Repositories", ns => ns
        .AddType("UserRepository", type => type
            .AddCommand("RetrieveAsync", cmd => cmd
                .UseCommandText("SELECT * FROM Users WHERE Id = @id")
                .UseConnectionAlias("Default"))))
    .Build();
```

## Performance and Threading

### Thread Safety

The Syrx.Commanders.Databases framework is **fully thread-safe** across all components:

- **Runtime Components**: `DatabaseCommander<T>`, `DatabaseConnector`, and `DatabaseCommandReader` use `ConcurrentDictionary` for caching and maintain no mutable shared state
- **Builder Classes**: `CommanderSettingsBuilder`, `NamespaceSettingBuilder`, and `TypeSettingBuilder` use `ConcurrentDictionary` internally and are safe for concurrent use
- **Configuration Models**: All settings are immutable records after construction
- **Service Lifetimes**: Can be safely registered as Singleton, Scoped, or Transient

### Performance Optimizations

#### Command Caching
Commands are cached using thread-safe `ConcurrentDictionary` with the pattern `{TypeFullName}.{MethodName}`:

```csharp
// Cached lookup - subsequent calls are extremely fast
var result = await _commander.QueryAsync<User>(new { id = 123 });
```

#### Connection String Caching
Connection strings are cached by alias to avoid repeated LINQ operations:

```csharp
// First call: Searches settings collection
// Subsequent calls: Retrieved from cache
var connection = _connector.CreateConnection(commandSetting);
```

#### Recommended Service Lifetimes
```csharp
// Recommended: Single instance with cached command resolution
services.AddSingleton<ICommander<UserRepository>, DatabaseCommander<UserRepository>>();

// Alternative: Scoped for web applications
services.AddScoped<ICommander<UserRepository>, DatabaseCommander<UserRepository>>();
```

#### Best Practices for High Performance
1. **Use Async Methods**: Prefer `QueryAsync` and `ExecuteAsync` for non-blocking operations
2. **Connection Pooling**: Configure appropriate connection pool sizes in connection strings
3. **Command Timeouts**: Set realistic timeouts to avoid unnecessary blocking
4. **Parameter Objects**: Use strongly-typed parameter objects for better performance

```csharp
// Good: Strongly-typed parameters
await _commander.QueryAsync<User>(new { id = 123, active = true });

// Avoid: Anonymous objects with complex expressions
await _commander.QueryAsync<User>(new { id = users.Where(u => u.Active).First().Id });
```

## Advanced Topics

### Multi-mapping Queries

Handle complex object relationships:

```csharp
// Two-table join
public async Task<IEnumerable<User>> RetrieveWithProfilesAsync(CancellationToken cancellationToken = default)
{
    return await _commander.QueryAsync<User, Profile, User>(
        (user, profile) => 
        {
            user.Profile = profile;
            return user;
    },
    cancellationToken: cancellationToken);
}

// Configure split column in settings
{
  "SplitOn": "ProfileId"
}
```

### Multiple Result Sets

Process stored procedures returning multiple result sets:

```csharp
public async Task<DashboardData> RetrieveDashboardAsync(int userId, CancellationToken cancellationToken = default)
{
    var (users, orders, notifications) = await _commander
  .QueryMultipleAsync<User, Order, Notification>(new { userId });
    
    return new DashboardData
    {
        User = users.FirstOrDefault(),
        Orders = orders.ToList(),
        Notifications = notifications.ToList()
    };
}
```

### Connection Strategies

#### Read/Write Separation
```json
{
  "Connections": [
    {
      "Alias": "Primary",
      "ConnectionString": "Server=write-db;..."
    },
    {
      "Alias": "ReadOnly", 
      "ConnectionString": "Server=read-db;..."
    }
  ]
}
```

```csharp
// Use different connections for different operations
{
  "RetrieveUsersAsync": {
    "ConnectionAlias": "ReadOnly"
  },
  "CreateUserAsync": {
    "ConnectionAlias": "Primary"
  }
}
```

### Custom Command Types

#### Stored Procedures
```json
{
  "ProcessOrderAsync": {
    "CommandText": "sp_ProcessOrder",
    "CommandType": "StoredProcedure",
    "CommandTimeout": 300
  }
}
```

#### Table Direct Access
```json
{
  "RetrieveUserTableAsync": {
    "CommandText": "Users",
    "CommandType": "TableDirect"
  }
}
```

## Performance Considerations

### Connection Pooling
- Utilizes ADO.NET connection pooling automatically
- Configure pool settings in connection strings:
  ```
  "Server=localhost;Database=MyApp;Pooling=true;Max Pool Size=100;Min Pool Size=5;"
  ```

### Command Caching
- DatabaseCommander caches `CommandSetting` lookups per method
- Eliminates repeated LINQ operations on settings collections
- Uses `ConcurrentDictionary` for thread-safe caching

### Dapper Optimizations
- Leverages Dapper's compiled query caching
- Uses buffered queries by default for better performance
- Supports unbuffered queries for large result sets when needed

### Best Practices

1. **Use Appropriate Connection Lifetimes**
   ```csharp
   // Scoped for web applications
   services.AddScoped<UserRepository>();
   
   // Singleton for cached data
   services.AddSingleton<ConfigurationRepository>();
   ```

2. **Optimize Query Parameters**
   ```csharp
   // Good: Use specific parameters
   await _commander.QueryAsync<User>(new { id, isActive = true });
   
   // Avoid: Passing entire objects as parameters when not needed
   ```

3. **Configure Appropriate Timeouts**
   ```json
   {
     "CommandTimeout": 30,      // Default queries
     "CommandTimeout": 300      // Long-running operations
   }
   ```

## Migration Guide

### From Version 2.x to 3.x

1. **Package References**
   - Update all package references to 3.x versions
   - No breaking changes in public APIs

2. **Configuration Changes**
   - Configuration schema remains compatible
   - New optional properties available

3. **Service Registration**
   - Registration patterns remain the same
   - New extension methods available

### From Other ORMs

#### From Entity Framework
```csharp
// Entity Framework
public async Task<User> RetrieveUserAsync(int id)
{
    return await _context.Users.FindAsync(id);
}

// Syrx
public async Task<User> RetrieveUserAsync(int id)
{
    var result = await _commander.QueryAsync<User>(new { id });
    return result.FirstOrDefault();
}
```

#### From Raw ADO.NET
```csharp
// Raw ADO.NET
public async Task<User> RetrieveUserAsync(int id)
{
    using var connection = new SqlConnection(connectionString);
    using var command = new SqlCommand("SELECT * FROM Users WHERE Id = @id", connection);
    command.Parameters.AddWithValue("@id", id);
    // ... manual mapping
}

// Syrx
public async Task<User> RetrieveUserAsync(int id)
{
    var result = await _commander.QueryAsync<User>(new { id });
    return result.FirstOrDefault();
}
```

## API Reference

### Core Interfaces

- **[ICommander&lt;TRepository&gt;](../src/Syrx.Commanders.Databases/README.md#icommander)**: Primary interface for repository operations
- **[IDatabaseConnector](../src/Syrx.Commanders.Databases.Connectors/README.md#idatabaseconnector)**: Database connection creation
- **[ICommanderSettings](../src/Syrx.Commanders.Databases.Settings/README.md#icommandersettings)**: Configuration container

### Core Classes

- **[DatabaseCommander&lt;TRepository&gt;](../src/Syrx.Commanders.Databases/README.md#databasecommander)**: Main command executor
- **[DatabaseConnector](../src/Syrx.Commanders.Databases.Connectors/README.md#databaseconnector)**: Connection management
- **[CommanderSettings](../src/Syrx.Commanders.Databases.Settings/README.md#commandersettings)**: Configuration model

### Configuration Models

- **[CommandSetting](../src/Syrx.Commanders.Databases.Settings/README.md#commandsetting)**: Individual command configuration
- **[ConnectionStringSetting](../src/Syrx.Commanders.Databases.Settings/README.md#connectionstringsetting)**: Connection string configuration
- **[NamespaceSetting](../src/Syrx.Commanders.Databases.Settings/README.md#namespacesetting)**: Namespace-level configuration

## Examples

For complete working examples, see:

- **[Basic CRUD Operations](examples/basic-crud.md)**
- **[Multi-mapping Queries](examples/multi-mapping.md)**
- **[Multiple Result Sets](examples/multiple-results.md)**
- **[Transaction Management](examples/transactions.md)**
- **[Configuration Patterns](examples/configuration.md)**
- **[Performance Optimization](examples/performance.md)**

## Contributing

See the [Contributing Guide](../CONTRIBUTING.md) for information about:
- Development setup
- Coding standards
- Pull request process
- Testing requirements

## Support

- **Documentation**: [Complete documentation](./)
- **Issues**: [GitHub Issues](https://github.com/Syrx/Syrx.Commanders.Databases/issues)
- **Discussions**: [GitHub Discussions](https://github.com/Syrx/Syrx.Commanders.Databases/discussions)

## License

This project is licensed under the [MIT License](../LICENSE).



