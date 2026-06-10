# API Reference

**This page is now supplemented by comprehensive reference documentation.**

👉 **Start here**: [Complete Reference Documentation](reference/index.md)

---

## Quick Navigation

- **New Users**: [Getting Started Guide](reference/getting-started.md)
- **Architecture Deep-Dives**: [Architecture Overview](reference/architecture/index.md)
- **Configuration Reference**: [All Formats](reference/configuration/index.md)
- **Project-by-Project Docs**: [Project Reference](reference/projects/index.md)
- **Coverage & Metrics**: [Coverage Report](reference/coverage-report.md)

---

## Legacy API Quick Reference

Below is the original quick reference. For comprehensive, up-to-date API documentation, **visit the [Reference Documentation](reference/index.md) instead**.

## Core Interfaces

### ICommander&lt;TRepository&gt;

The primary interface used by application repositories to execute database commands.

```csharp
public interface ICommander<TRepository>
{
    // Query operations
    IEnumerable<TResult> Query<TResult>(object parameters = null, [CallerMemberName] string method = null);
    Task<IEnumerable<TResult>> QueryAsync<TResult>(object parameters = null, CancellationToken cancellationToken = default, [CallerMemberName] string method = null);
    
    // Multi-mapping query operations
    IEnumerable<TResult> Query<T1, T2, TResult>(Func<T1, T2, TResult> map, object parameters = null, [CallerMemberName] string method = null);
    Task<IEnumerable<TResult>> QueryAsync<T1, T2, TResult>(Func<T1, T2, TResult> map, object parameters = null, CancellationToken cancellationToken = default, [CallerMemberName] string method = null);
    
    // Multiple result set operations
    IEnumerable<TResult> Query<T1, TResult>(Func<IEnumerable<T1>, IEnumerable<TResult>> map, object parameters = null, [CallerMemberName] string method = null);
    Task<IEnumerable<TResult>> QueryAsync<T1, TResult>(Func<IEnumerable<T1>, IEnumerable<TResult>> map, object parameters = null, CancellationToken cancellationToken = default, [CallerMemberName] string method = null);
    
    // Execute operations
    bool Execute<TResult>(TResult model, [CallerMemberName] string method = null);
    Task<bool> ExecuteAsync<TResult>(TResult model, CancellationToken cancellationToken = default, [CallerMemberName] string method = null);
}
```

#### Key Methods

- **Query&lt;TResult&gt;**: Execute queries that return collections of objects
- **QueryAsync&lt;TResult&gt;**: Asynchronous version of Query
- **Execute&lt;TResult&gt;**: Execute commands that modify data (INSERT, UPDATE, DELETE)
- **ExecuteAsync&lt;TResult&gt;**: Asynchronous version of Execute

### IDatabaseConnector

Interface for creating database connections based on command settings.

```csharp
public interface IDatabaseConnector : IConnector<IDbConnection, CommandSetting>
{
    IDbConnection CreateConnection(CommandSetting setting);
}
```

### ICommanderSettings

Interface defining the configuration structure for the framework.

```csharp
public interface ICommanderSettings
{
    List<ConnectionStringSetting>? Connections { get; init; }
    List<NamespaceSetting> Namespaces { get; init; }
}
```

## Core Classes

### DatabaseCommander&lt;TRepository&gt;

The main implementation of `ICommander<TRepository>` that handles database command execution.

```csharp
public partial class DatabaseCommander<TRepository> : ICommander<TRepository>
{
    public DatabaseCommander(IDatabaseCommandReader reader, IDatabaseConnector connector);
    
    // Implements all ICommander<TRepository> methods
    // Handles connection management
    // Manages transactions for execute operations
    // Resolves commands from configuration
}
```

#### Key Features

- **Command Resolution**: Automatically resolves commands based on calling method name
- **Connection Management**: Creates and manages database connections
- **Transaction Handling**: Wraps execute operations in transactions
- **Multi-mapping Support**: Handles complex object relationships
- **Multiple Result Sets**: Processes stored procedures with multiple results

### DatabaseConnector

Default implementation of `IDatabaseConnector`.

```csharp
public class DatabaseConnector : IDatabaseConnector
{
    public DatabaseConnector(ICommanderSettings settings, Func<DbProviderFactory> providerPredicate);
    
    public IDbConnection CreateConnection(CommandSetting setting);
}
```

#### Key Features

- **Provider Factory Pattern**: Uses factory functions for provider-specific connections
- **Connection String Resolution**: Resolves connection strings by alias
- **Caching**: Caches connection string lookups for performance

## Configuration Models

### CommandSetting

Represents the configuration for a single database command.

```csharp
public sealed record CommandSetting : ICommandSetting
{
    public string Split { get; init; } = "Id";
    public required string CommandText { get; init; }
    public int CommandTimeout { get; init; } = 30;
    public CommandType CommandType { get; init; } = CommandType.Text;
    public CommandFlagSetting Flags { get; init; } = CommandFlagSetting.Buffered | CommandFlagSetting.NoCache;
    public IsolationLevel IsolationLevel { get; init; } = IsolationLevel.Serializable;
    public required string ConnectionAlias { get; init; }
}
```

#### Properties

- **CommandText**: The SQL command to execute
- **ConnectionAlias**: Reference to a named connection string
- **CommandTimeout**: Command timeout in seconds (default: 30)
- **CommandType**: Type of command (Text, StoredProcedure, TableDirect)
- **Split**: Column name(s) for multi-mapping queries (default: "Id")
- **IsolationLevel**: Transaction isolation level for execute operations

### ConnectionStringSetting

Represents a named connection string configuration.

```csharp
public sealed record ConnectionStringSetting
{
    public required string ConnectionString { get; init; }
    public required string Alias { get; init; }
}
```

### NamespaceSetting

Represents namespace-level configuration containing types and their commands.

```csharp
public sealed record NamespaceSetting
{
    public required string Namespace { get; init; }
    public required List<TypeSetting> Types { get; init; }
}
```

### TypeSetting

Represents type-level configuration containing command mappings.

```csharp
public sealed record TypeSetting
{
    public required string Name { get; init; }
    public required Dictionary<string, CommandSetting> Commands { get; init; }
}
```

## Extension Methods

### ServiceCollectionExtensions

Extension methods for registering Syrx services in the DI container.

```csharp
public static class ServiceCollectionExtensions
{
    // Register database commanders
    public static IServiceCollection AddDatabaseCommander<T>(
        this IServiceCollection services, 
        ServiceLifetime lifetime = ServiceLifetime.Scoped);
    
    // Register database connectors
    public static IServiceCollection AddDatabaseConnector<TConnector>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TConnector : class, IDatabaseConnector;
}
```

### Configuration Extensions

#### JSON Configuration

```csharp
public static class UseFileExtensions
{
    public static SyrxBuilder UseFile(
        this SyrxBuilder factory, 
        string fileName, 
        IConfigurationBuilder builder);
}
```

#### Builder Pattern

```csharp
public static class CommanderSettingsBuilderExtensions
{
    public static CommanderSettingsBuilder AddConnectionString(
        this CommanderSettingsBuilder builder,
        string alias,
        string connectionString);
        
    public static CommanderSettingsBuilder AddNamespace(
        this CommanderSettingsBuilder builder,
        string name,
        Action<NamespaceSettingBuilder> configure);
}
```

## Builder Classes

### CommanderSettingsBuilder

Fluent builder for creating `CommanderSettings` instances. **Thread-safe** - uses `ConcurrentDictionary` internally and can be safely used across multiple threads.

```csharp
public class CommanderSettingsBuilder
{
    public CommanderSettingsBuilder AddConnectionString(string alias, string connectionString);
    public CommanderSettingsBuilder AddNamespace(string name, Action<NamespaceSettingBuilder> configure);
    public CommanderSettings Build();
}
```

### CommandSettingBuilder

Fluent builder for creating `CommandSetting` instances.

```csharp
public class CommandSettingBuilder
{
    public CommandSettingBuilder UseCommandText(string commandText);
    public CommandSettingBuilder UseConnectionAlias(string alias);
    public CommandSettingBuilder SetCommandTimeout(int timeout);
    public CommandSettingBuilder SetCommandType(CommandType type);
    public CommandSettingBuilder SplitOn(string splitOn);
    public CommandSettingBuilder SetIsolationLevel(IsolationLevel level);
    public CommandSetting Build();
}
```

## Schema Modeling (Builders Package)

### Database

Represents a database schema definition.

```csharp
public class Database
{
    public string Name { get; }
    public IEnumerable<Table> Tables { get; }
    
    public Database(string name, IEnumerable<Table> tables);
}
```

### Table

Represents a database table definition.

```csharp
public class Table
{
    public string Name { get; }
    public IEnumerable<Field> Fields { get; }
    
    public Table(string name, IEnumerable<Field> fields);
}
```

### Field

Represents a database field/column definition.

```csharp
public class Field
{
    public string Name { get; }
    public Type DataType { get; }
    
    public Field(string name, Type dataType);
}
```

## Enumerations

### CommandType

Specifies the type of database command.

```csharp
public enum CommandType
{
    Text = 1,           // SQL text command
    StoredProcedure = 4, // Stored procedure
    TableDirect = 512    // Direct table access
}
```

### IsolationLevel

Specifies the transaction isolation level.

```csharp
public enum IsolationLevel
{
    Unspecified = -1,
    Chaos = 16,
    ReadUncommitted = 256,
    ReadCommitted = 4096,
    RepeatableRead = 65536,
    Serializable = 1048576,
    Snapshot = 16777216
}
```

### CommandFlagSetting

Flags that control command execution behavior.

```csharp
[Flags]
public enum CommandFlagSetting
{
    None = 0,
    Buffered = 1,
    Pipelined = 2,
    NoCache = 4
}
```

## Usage Patterns

### Repository Pattern Implementation

```csharp
public class UserRepository
{
    private readonly ICommander<UserRepository> _commander;
    
    public UserRepository(ICommander<UserRepository> commander)
    {
        _commander = commander;
    }
    
    // Simple query
    public async Task<User> RetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = await _commander.QueryAsync<User>(new { id }, cancellationToken);
        return result.FirstOrDefault();
    }
    
    // Multi-mapping query
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
    
    // Execute operation
    public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        return await _commander.ExecuteAsync(user, cancellationToken) ? user : null;
    }
}
```

### Configuration Patterns

#### Programmatic Configuration (Recommended)

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

#### JSON Configuration (Optional)
```json
{
  "Connections": [
    {
      "Alias": "Default",
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
              "ConnectionAlias": "Default"
            }
          }
        }
      ]
    }
  ]
}
```

### Service Registration Patterns

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Method 1 (recommended): using programmatic configuration
    services.AddSingleton<ICommanderSettings>(settings);
    services.AddDatabaseCommander<UserRepository>();
    services.AddDatabaseConnector<SqlServerDatabaseConnector>();

    // Method 2 (optional): using JSON configuration
    var configBuilder = new ConfigurationBuilder();
    services.UseSyrx(builder => builder
        .UseFile("syrx.json", configBuilder));
    
    // Register repositories
    services.AddScoped<UserRepository>();
}
```

## Error Handling

### Common Exceptions

- **InvalidOperationException**: Command configuration not found
- **ArgumentNullException**: Required parameters are null
- **SqlException**: Database-specific errors
- **TimeoutException**: Command timeout exceeded
- **OperationCanceledException**: Operation was cancelled

### Exception Handling Patterns

```csharp
try
{
    var result = await _commander.QueryAsync<User>(new { id });
}
catch (InvalidOperationException ex)
{
    // Command not configured or connection alias not found
    _logger.LogError(ex, "Configuration error for command resolution");
}
catch (SqlException ex)
{
    // Database-specific error
    _logger.LogError(ex, "Database error occurred");
}
catch (TimeoutException ex)
{
    // Command timeout
    _logger.LogWarning(ex, "Command execution timed out");
}
```

## Performance Considerations

### Caching

- **Command Settings**: Cached by method name using `ConcurrentDictionary`
- **Connection Strings**: Cached by alias using `ConcurrentDictionary`
- **Dapper Query Plans**: Automatically cached by Dapper

### Connection Management

- Utilizes ADO.NET connection pooling
- Connections are created per command execution
- No connection state is maintained between operations

### Best Practices

1. Use appropriate service lifetimes (Scoped for web apps)
2. Configure reasonable command timeouts
3. Use parameters for all dynamic values
4. Consider unbuffered queries for large result sets
5. Use async methods consistently

For more detailed information, see the individual package documentation linked throughout this reference.



