````markdown
# Syrx.Commanders.Databases.Connectors.Extensions

Dependency injection extensions for Syrx database connectors.

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Installation](#installation)
- [Extension Methods](#extension-methods)
- [Usage](#usage)
  - [Basic Registration](#basic-registration)
  - [Service Lifetime Configuration](#service-lifetime-configuration)
  - [Multiple Connectors](#multiple-connectors)
- [Connector Registration](#connector-registration)
  - [SQL Server](#sql-server)
  - [MySQL](#mysql)
  - [PostgreSQL](#postgresql)
  - [Oracle](#oracle)
- [Advanced Scenarios](#advanced-scenarios)
  - [Named Connectors](#named-connectors)
  - [Conditional Registration](#conditional-registration)
  - [Factory Patterns](#factory-patterns)
- [Integration Examples](#integration-examples)
  - [ASP.NET Core](#aspnet-core)
  - [Console Applications](#console-applications)
- [Related Packages](#related-packages)
- [License](#license)
- [Credits](#credits)

## Overview

`Syrx.Commanders.Databases.Connectors.Extensions` provides dependency injection extension methods for registering database connectors in the Microsoft.Extensions.DependencyInjection container. This package simplifies the registration and configuration of database connectors for the Syrx framework.

## Key Features

- **Easy Registration**: Simple extension methods for connector registration
- **Service Lifetime Management**: Control over connector lifetimes (Singleton, Scoped, Transient)
- **Multiple Connector Support**: Register multiple connectors for different databases
- **Type Safety**: Strongly-typed registration methods
- **Integration Ready**: Seamless integration with ASP.NET Core and other .NET applications

## Installation

```bash
dotnet add package Syrx.Commanders.Databases.Connectors.Extensions
```

**Package Manager**
```bash
Install-Package Syrx.Commanders.Databases.Connectors.Extensions
```

**PackageReference**
```xml
<PackageReference Include="Syrx.Commanders.Databases.Connectors.Extensions" Version="3.0.0" />
```

## Extension Methods

Key extension methods for registering database connectors:

```csharp
public static class ServiceCollectionExtensions
{
    // Register a database connector with default lifetime (Scoped)
    public static IServiceCollection AddDatabaseConnector<TConnector>(
        this IServiceCollection services)
        where TConnector : class, IDatabaseConnector;
    
    // Register a database connector with specified lifetime
    public static IServiceCollection AddDatabaseConnector<TConnector>(
        this IServiceCollection services,
        ServiceLifetime lifetime)
        where TConnector : class, IDatabaseConnector;
    
    // Register a database connector with factory
    public static IServiceCollection AddDatabaseConnector<TConnector>(
        this IServiceCollection services,
        Func<IServiceProvider, TConnector> factory,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TConnector : class, IDatabaseConnector;
}

public static class DatabaseConnectorExtensions
{
    // Extension methods for IDatabaseConnector instances
    public static IDatabaseConnector WithConnectionString(
        this IDatabaseConnector connector,
        string alias,
        string connectionString);
}
```

## Usage

### Basic Registration

```csharp
using Syrx.Commanders.Databases.Connectors.Extensions;

public void ConfigureServices(IServiceCollection services)
{
    // Register SQL Server connector
    services.AddDatabaseConnector<SqlServerDatabaseConnector>();
    
    // Register with explicit interface
    services.AddDatabaseConnector<SqlServerDatabaseConnector>();
    
    // The connector will be available as IDatabaseConnector
}
```

### Service Lifetime Configuration

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Scoped lifetime (default) - one instance per request/scope
    services.AddDatabaseConnector<SqlServerDatabaseConnector>(ServiceLifetime.Scoped);
    
    // Singleton lifetime - one instance for application lifetime
    services.AddDatabaseConnector<SqlServerDatabaseConnector>(ServiceLifetime.Singleton);
    
    // Transient lifetime - new instance every time
    services.AddDatabaseConnector<SqlServerDatabaseConnector>(ServiceLifetime.Transient);
}
```

### Multiple Connectors

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Register multiple connectors for different databases
    services.AddDatabaseConnector<SqlServerDatabaseConnector>();
    services.AddDatabaseConnector<MySqlDatabaseConnector>();
    services.AddDatabaseConnector<NpgsqlDatabaseConnector>();
    
    // Use named services or factories to resolve specific connectors
}
```

## Connector Registration

### SQL Server

```csharp
// Add SQL Server packages first
// dotnet add package Syrx.Commanders.Databases.Connectors.SqlServer

using Syrx.Commanders.Databases.Connectors.SqlServer;

public void ConfigureServices(IServiceCollection services)
{
    services.AddDatabaseConnector<SqlServerDatabaseConnector>();
    
    // Or with factory for custom configuration
    services.AddDatabaseConnector<SqlServerDatabaseConnector>(provider =>
        new SqlServerDatabaseConnector(/* custom parameters */));
}
```

### MySQL

```csharp
// Add MySQL packages first  
// dotnet add package Syrx.Commanders.Databases.Connectors.MySql

using Syrx.Commanders.Databases.Connectors.MySql;

public void ConfigureServices(IServiceCollection services)
{
    services.AddDatabaseConnector<MySqlDatabaseConnector>();
}
```

### PostgreSQL

```csharp
// Add PostgreSQL packages first
// dotnet add package Syrx.Commanders.Databases.Connectors.Npgsql

using Syrx.Commanders.Databases.Connectors.Npgsql;

public void ConfigureServices(IServiceCollection services)
{
    services.AddDatabaseConnector<NpgsqlDatabaseConnector>();
}
```

### Oracle

```csharp
// Add Oracle packages first
// dotnet add package Syrx.Commanders.Databases.Connectors.Oracle

using Syrx.Commanders.Databases.Connectors.Oracle;

public void ConfigureServices(IServiceCollection services)
{
    services.AddDatabaseConnector<OracleDatabaseConnector>();
}
```

## Advanced Scenarios

### Named Connectors

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Register multiple connectors with different names
    services.AddKeyedScoped<IDatabaseConnector, SqlServerDatabaseConnector>("SqlServer");
    services.AddKeyedScoped<IDatabaseConnector, MySqlDatabaseConnector>("MySQL");
    services.AddKeyedScoped<IDatabaseConnector, NpgsqlDatabaseConnector>("PostgreSQL");
}

// Usage in a service
public class MultiDatabaseService
{
    private readonly IDatabaseConnector _sqlServerConnector;
    private readonly IDatabaseConnector _mySqlConnector;
    
    public MultiDatabaseService(
        [FromKeyedServices("SqlServer")] IDatabaseConnector sqlServerConnector,
        [FromKeyedServices("MySQL")] IDatabaseConnector mySqlConnector)
    {
        _sqlServerConnector = sqlServerConnector;
        _mySqlConnector = mySqlConnector;
    }
}
```

### Conditional Registration

```csharp
public void ConfigureServices(IServiceCollection services)
{
    var databaseProvider = Configuration.GetValue<string>("DatabaseProvider");
    
    switch (databaseProvider?.ToLower())
    {
        case "sqlserver":
            services.AddDatabaseConnector<SqlServerDatabaseConnector>();
            break;
        case "mysql":
            services.AddDatabaseConnector<MySqlDatabaseConnector>();
            break;
        case "postgresql":
            services.AddDatabaseConnector<NpgsqlDatabaseConnector>();
            break;
        default:
            throw new InvalidOperationException($"Unsupported database provider: {databaseProvider}");
    }
}
```

### Factory Patterns

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Register with custom factory
    services.AddDatabaseConnector<SqlServerDatabaseConnector>(provider =>
    {
        var configuration = provider.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        return new SqlServerDatabaseConnector(/* custom initialization */);
    });
    
    // Register factory that can create different connectors
    services.AddScoped<IDatabaseConnectorFactory, DatabaseConnectorFactory>();
}

public interface IDatabaseConnectorFactory
{
    IDatabaseConnector CreateConnector(string providerName);
}

public class DatabaseConnectorFactory : IDatabaseConnectorFactory
{
    private readonly IServiceProvider _serviceProvider;
    
    public DatabaseConnectorFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public IDatabaseConnector CreateConnector(string providerName)
    {
        return providerName.ToLower() switch
        {
            "sqlserver" => _serviceProvider.GetRequiredService<SqlServerDatabaseConnector>(),
            "mysql" => _serviceProvider.GetRequiredService<MySqlDatabaseConnector>(),
            "postgresql" => _serviceProvider.GetRequiredService<NpgsqlDatabaseConnector>(),
            _ => throw new ArgumentException($"Unknown provider: {providerName}")
        };
    }
}
```

## Integration Examples

### ASP.NET Core

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Register database connector
builder.Services.AddDatabaseConnector<SqlServerDatabaseConnector>();

// Register other Syrx services
builder.Services.AddScoped<ICommander<UserRepository>, DatabaseCommander<UserRepository>>();

// Register your repositories
builder.Services.AddScoped<UserRepository>();

var app = builder.Build();
```

### Console Applications

```csharp
class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        
        // Register connector
        services.AddDatabaseConnector<SqlServerDatabaseConnector>();
        
        // Register other services
        services.AddScoped<ICommander<DataProcessor>, DatabaseCommander<DataProcessor>>();
        services.AddScoped<DataProcessor>();
        
        var provider = services.BuildServiceProvider();
        
        // Use the service
        var processor = provider.GetRequiredService<DataProcessor>();
        await processor.ProcessDataAsync();
    }
}
```

## Related Packages

### Database-Specific Connectors
- **[Syrx.Commanders.Databases.Connectors.SqlServer](https://www.nuget.org/packages/Syrx.Commanders.Databases.Connectors.SqlServer/)**: SQL Server connector implementation
- **[Syrx.Commanders.Databases.Connectors.MySql](https://www.nuget.org/packages/Syrx.Commanders.Databases.Connectors.MySql/)**: MySQL connector implementation
- **[Syrx.Commanders.Databases.Connectors.Npgsql](https://www.nuget.org/packages/Syrx.Commanders.Databases.Connectors.Npgsql/)**: PostgreSQL connector implementation
- **[Syrx.Commanders.Databases.Connectors.Oracle](https://www.nuget.org/packages/Syrx.Commanders.Databases.Connectors.Oracle/)**: Oracle connector implementation

### Core Framework
- **[Syrx.Commanders.Databases.Connectors](https://www.nuget.org/packages/Syrx.Commanders.Databases.Connectors/)**: Connector abstractions and base classes
- **[Syrx.Commanders.Databases](https://www.nuget.org/packages/Syrx.Commanders.Databases/)**: Database command abstractions
- **[Syrx.Commanders.Databases.Extensions](https://www.nuget.org/packages/Syrx.Commanders.Databases.Extensions/)**: General database extensions

## License

This project is licensed under the [MIT License](https://github.com/Syrx/Syrx/blob/main/LICENSE).

## Credits

Built on top of [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/) and [Dapper](https://github.com/DapperLib/Dapper).
````



