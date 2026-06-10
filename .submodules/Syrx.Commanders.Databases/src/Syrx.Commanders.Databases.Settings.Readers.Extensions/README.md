# Syrx.Commanders.Databases.Settings.Readers.Extensions

Dependency injection extensions for internal Syrx command readers.

## Overview

This package provides extension methods to register `IDatabaseCommandReader` in the dependency injection container. Note that `IDatabaseCommandReader` is an **internal component** used by `DatabaseCommander<TRepository>` - developers work with `ICommander<TRepository>` in their application code.

## Installation

```bash
dotnet add package Syrx.Commanders.Databases.Settings.Readers.Extensions
```

**Package Manager**
```bash
Install-Package Syrx.Commanders.Databases.Settings.Readers.Extensions
```

**PackageReference**
```xml
<PackageReference Include="Syrx.Commanders.Databases.Settings.Readers.Extensions" Version="3.0.0" />
```

## Extension Methods

### AddReader

Registers the internal `IDatabaseCommandReader` service used by the framework:

```csharp
public static IServiceCollection AddReader(
    this IServiceCollection services,
    ServiceLifetime lifetime = ServiceLifetime.Transient)
```

## Framework Usage

This extension is typically used internally by database provider packages:

```csharp
// Used internally by Syrx framework setup
services.AddReader(ServiceLifetime.Scoped);
```

## Typical Application Setup

In most cases, you don't call this directly. Instead, use the high-level Syrx configuration:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // This automatically registers all internal services including IDatabaseCommandReader
    services.UseSyrx(builder => builder
        .UseSqlServer(sqlServer => sqlServer
            .AddConnectionString("DefaultConnection", connectionString)
            .AddCommand<UserRepository>(commands => commands
                .ForMethod(nameof(UserRepository.RetrieveAsync), command => command
                    .UseConnectionAlias("DefaultConnection")
                    .UseCommandText("SELECT * FROM Users WHERE Id = @id")))));
    
    // Register your repositories that use ICommander<T>
    services.AddScoped<UserRepository>();
}
```

## Repository Implementation

Your repositories use `ICommander<TRepository>`, not the internal reader:

```csharp
public class UserRepository
{
    private readonly ICommander<UserRepository> _commander;
    
    public UserRepository(ICommander<UserRepository> commander)
    {
        _commander = commander;
    }
    
    public async Task<User> RetrieveAsync(int id, CancellationToken cancellationToken = default)
        => (await _commander.QueryAsync<User>(new { id }, cancellationToken)).FirstOrDefault();
        
    public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default)
        => await _commander.ExecuteAsync(user, cancellationToken) ? user : default;
}
```

## Architecture Context

This package fits into the Syrx architecture as follows:

```
Application Code:
  Repository → ICommander<T>
    ↓
Framework Internal:
  DatabaseCommander<T> → IDatabaseCommandReader ← This package
    ↓
Execution:
  Dapper → Database
```

## Dependencies

This package depends on:
- **Syrx.Commanders.Databases.Settings.Readers** - Core reader interfaces and implementations
- **Syrx.Extensions** - Base extension utilities
- **Microsoft.Extensions.DependencyInjection.Abstractions** - Dependency injection abstractions

## License

This project is licensed under the [MIT License](https://github.com/Syrx/Syrx/blob/main/LICENSE).

## Credits

Provides dependency injection support for internal Syrx framework components.



