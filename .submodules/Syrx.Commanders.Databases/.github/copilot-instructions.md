# Copilot Instructions for Syrx.Commanders.Databases

This document provides guidance for GitHub Copilot and other AI assistants working with the Syrx.Commanders.Databases codebase.

## Project Overview

Syrx.Commanders.Databases is a high-performance database access framework built on top of Dapper. It provides configuration-driven database command execution with strong typing, multi-provider support, and automatic transaction management.

## Architecture Principles

### Core Design Patterns

1. **Repository Pattern**: Application repositories inject `ICommander<TRepository>` for database operations
2. **Configuration-Driven**: SQL commands are externalized from code using JSON/XML configuration
3. **Command Resolution**: Methods are automatically mapped to commands using `{Namespace}.{ClassName}.{MethodName}` pattern
4. **Provider Abstraction**: Database providers are abstracted through `IDatabaseConnector` interface
5. **Partial Classes**: Large classes like `DatabaseCommander<T>` are split across multiple files by functionality

### Project Structure

```
src/
├── Syrx.Commanders.Databases/                    # Core command execution
├── Syrx.Commanders.Databases.Connectors/         # Connection abstractions
├── Syrx.Commanders.Databases.Connectors.Extensions/ # DI extensions for connectors
├── Syrx.Commanders.Databases.Extensions/         # General DI extensions
├── Syrx.Commanders.Databases.Settings/           # Configuration models
├── Syrx.Commanders.Databases.Settings.Extensions/ # Builder pattern for settings
├── Syrx.Commanders.Databases.Settings.Extensions.Json/ # JSON configuration
├── Syrx.Commanders.Databases.Settings.Extensions.Xml/  # XML configuration
├── Syrx.Commanders.Databases.Settings.Readers/   # Internal command resolution
├── Syrx.Commanders.Databases.Settings.Readers.Extensions/ # DI for readers
└── Syrx.Commanders.Databases.Builders/           # Schema modeling utilities
```

## Coding Standards

### C# Style Guidelines

1. **XML Documentation**: All public interfaces, classes, and methods MUST have comprehensive XML documentation
   ```csharp
   /// <summary>
   /// Executes a database command and returns the number of affected rows.
   /// </summary>
   /// <param name="model">The model containing command parameters.</param>
   /// <param name="method">The calling method name (auto-populated).</param>
   /// <returns>True if one or more rows were affected; otherwise, false.</returns>
   /// <exception cref="ArgumentNullException">Thrown when model is null.</exception>
   public async Task<bool> ExecuteAsync<TResult>(TResult model, [CallerMemberName] string method = null)
   ```

2. **Nullable Reference Types**: The project uses nullable reference types - be explicit about nullability
   ```csharp
   public List<ConnectionStringSetting>? Connections { get; init; } // Nullable
   public required List<NamespaceSetting> Namespaces { get; init; } // Required
   ```

3. **Record Types**: Use record types for immutable configuration models
   ```csharp
   public sealed record CommandSetting : ICommandSetting
   {
       public required string CommandText { get; init; }
       public string ConnectionAlias { get; init; } = "Default";
   }
   ```

4. **CallerMemberName**: Use `[CallerMemberName]` for automatic method name resolution
   ```csharp
   public async Task<IEnumerable<T>> QueryAsync<T>(
       object parameters = null,
       [CallerMemberName] string method = null)
   ```

5. **Partial Classes**: Organize large classes across multiple files by functionality
   ```
   DatabaseCommander.cs              # Core class, constructor, disposal
   DatabaseCommander.Execute.cs      # Execute operations
   DatabaseCommander.ExecuteAsync.cs # Async execute operations
   DatabaseCommander.Query.*.cs      # Query operations
   ```

### Naming Conventions

- **Interfaces**: Start with `I` (e.g., `ICommander<T>`, `IDatabaseConnector`)
- **Generic Type Parameters**: Use descriptive names (e.g., `TRepository`, `TResult`)
- **Async Methods**: End with `Async` suffix
- **Extension Classes**: End with `Extensions` (e.g., `ServiceCollectionExtensions`)
- **Builder Classes**: End with `Builder` (e.g., `CommanderSettingsBuilder`)

### Error Handling Patterns

1. **Validation**: Use `Throw<T>` helper for argument validation
   ```csharp
   Throw<ArgumentNullException>(model != null, nameof(model));
   ```

2. **Database Exceptions**: Allow database-specific exceptions to bubble up
   ```csharp
   // Don't catch SqlException, TimeoutException, etc. - let consumers handle them
   ```

3. **Configuration Errors**: Throw `InvalidOperationException` for missing configuration
   ```csharp
   Throw<InvalidOperationException>(commandSetting != null, "Command not configured");
   ```

## Package Dependencies

### Core Dependencies
- **Dapper**: Micro-ORM for database operations (external)
- **Syrx**: Core interfaces and abstractions (submodule)
- **System.Data.Common**: ADO.NET abstractions

### Internal Dependencies
```
Syrx.Commanders.Databases (core)
├── Syrx.Commanders.Databases.Connectors
├── Syrx.Commanders.Databases.Settings
└── Syrx.Commanders.Databases.Settings.Readers

Extensions depend on their corresponding core packages
Builders package is standalone
```

## Key Interfaces and Implementations

### ICommander&lt;TRepository&gt;
- **Purpose**: Primary interface for repository database operations
- **Implementation**: `DatabaseCommander<TRepository>`
- **Usage**: Injected into repository classes
- **Methods**: Query, QueryAsync, Execute, ExecuteAsync with various overloads

### IDatabaseConnector
- **Purpose**: Creates database connections from command settings
- **Implementation**: `DatabaseConnector`
- **Key Feature**: Caches connection string lookups for performance

### ICommanderSettings
- **Purpose**: Configuration container for commands and connections
- **Implementation**: `CommanderSettings` record
- **Structure**: Hierarchical (Namespaces → Types → Commands)

## Configuration Architecture

### Command Resolution Pattern
```
Repository Method Call: UserRepository.RetrieveByIdAsync(int id)
                    ↓
Resolved Command Key: "MyApp.Repositories.UserRepository.RetrieveByIdAsync"
                    ↓
Command Configuration: { CommandText: "SELECT * FROM Users WHERE Id = @id", ... }
                    ↓
Database Execution: Execute with Dapper
```

### Configuration Sources
1. **Programmatic (recommended)**: Builder pattern via `Syrx.Commanders.Databases.Settings.Extensions`
2. **JSON**: File-based alternative using `UseFile()` extension
3. **XML**: File-based alternative using `UseFile()` extension

## Testing Patterns

### Repository Testing
```csharp
// Repositories should be tested with ICommander<T> mocks
var mockCommander = new Mock<ICommander<UserRepository>>();
mockCommander.Setup(x => x.QueryAsync<User>(It.IsAny<object>(), default, "RetrieveByIdAsync"))
            .ReturnsAsync(new[] { expectedUser });
```

### Integration Testing
```csharp
// Use real database with test configuration
services.UseSyrx(builder => builder.UseFile("test-syrx.json", configBuilder));
```

## Performance Considerations

### Caching Strategy
- **Command Settings**: Cached by `{TypeFullName}.{MethodName}` key
- **Connection Strings**: Cached by alias
- **Dapper**: Automatically caches compiled query plans

### Connection Management
- Use connection-per-operation pattern (no connection state)
- Leverage ADO.NET connection pooling
- Configure appropriate pool sizes in connection strings

### Transaction Handling
- **Queries**: No transaction (read-only operations)
- **Executes**: Automatic transaction with rollback on exception

## Extension Development

### Creating New Connectors
```csharp
public class CustomDatabaseConnector : IDatabaseConnector
{
    public IDbConnection CreateConnection(CommandSetting setting)
    {
        // Implement provider-specific connection creation
    }
}
```

### Adding Configuration Sources
```csharp
public static class CustomConfigurationExtensions
{
    public static SyrxBuilder UseCustomSource(this SyrxBuilder builder, CustomOptions options)
    {
        // Implement custom configuration loading
    }
}
```

## Documentation Standards

### README.md Files
- Each project MUST have a comprehensive README.md
- Follow the established template with ToC, Overview, Installation, Usage, etc.
- Include NuGet installation instructions and version badges
- Provide code examples for common scenarios

### XML Documentation
- All public APIs require comprehensive XML docs
- Include `<summary>`, `<param>`, `<returns>`, `<exception>` tags
- Use `<see cref=""/>` for cross-references
- Add `<remarks>` for additional context when helpful

### .docs Folder
- Comprehensive cross-project documentation
- Architecture overviews and design decisions
- API reference with usage examples
- Migration guides and best practices

## Common Patterns to Follow

### Service Registration
```csharp
// Standard DI registration pattern
services.UseSyrx(builder => builder.UseFile("syrx.json", configBuilder));
services.AddScoped<UserRepository>();
```

### Repository Implementation
```csharp
public class UserRepository
{
    private readonly ICommander<UserRepository> _commander;
    
    public UserRepository(ICommander<UserRepository> commander)
    {
        _commander = commander;
    }
    
    public async Task<User> RetrieveByIdAsync(int id)
    {
        var result = await _commander.QueryAsync<User>(new { id });
        return result.FirstOrDefault();
    }
}
```

### Configuration Structure
```json
{
  "Connections": [
    { "Alias": "Default", "ConnectionString": "..." }
  ],
  "Namespaces": [
    {
      "Name": "MyApp.Repositories",
      "Types": [
        {
          "Name": "UserRepository",
          "Commands": {
            "RetrieveByIdAsync": {
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

## Things to Avoid

1. **Don't expose internal interfaces**: `IDatabaseCommandReader` is internal
2. **Don't bypass the command resolution**: Always use `[CallerMemberName]`
3. **Don't cache connections**: Use connection-per-operation
4. **Don't catch database exceptions**: Let them bubble up for proper handling
5. **Don't mix sync/async**: Use async methods consistently
6. **Don't modify configuration at runtime**: Treat as immutable after startup

## Thread Safety

All components in the framework are **fully thread-safe**:

- **Core Runtime Components**: `DatabaseCommander<T>`, `DatabaseConnector`, `DatabaseCommandReader` use `ConcurrentDictionary` for caching and have no mutable shared state
- **Builder Classes**: `CommanderSettingsBuilder`, `NamespaceSettingBuilder`, `TypeSettingBuilder` use `ConcurrentDictionary` internally and are thread-safe
- **Configuration Models**: All settings classes are immutable records after construction
- **Caching Strategy**: Thread-safe caching using `ConcurrentDictionary.GetOrAdd()` and `TryAdd()` operations

## Debugging Tips

### Command Resolution Issues
- Check namespace/type/method name matching exactly
- Verify casing sensitivity in configuration
- Use debugger to inspect `GetCommandSetting()` calls

### Connection Problems
- Verify connection alias matches configuration
- Check connection string format for provider
- Ensure provider factory is registered correctly

### Performance Issues
- Enable SQL logging to see generated commands
- Check connection pool configuration
- Monitor command cache hit rates

## Future Considerations

When extending or modifying the framework:

1. **Backward Compatibility**: Maintain configuration schema compatibility
2. **Performance**: Consider cache impact of new features
3. **Provider Support**: Ensure new features work across database providers
4. **Thread Safety**: All components must be thread-safe
5. **Documentation**: Update all relevant documentation with changes

This framework is designed for high-performance, enterprise-grade database access. All changes should maintain these quality standards while preserving the simple, configuration-driven developer experience.
