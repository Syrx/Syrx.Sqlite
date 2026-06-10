````markdown
# Syrx.Commanders.Databases.Extensions

Dependency injection and service registration extensions for the Syrx database framework.

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Installation](#installation)
- [Extension Methods](#extension-methods)
  - [Service Registration](#service-registration)
  - [Configuration Extensions](#configuration-extensions)
- [Usage](#usage)
  - [Basic Service Registration](#basic-service-registration)
  - [Custom Service Lifetimes](#custom-service-lifetimes)
  - [Advanced Configuration](#advanced-configuration)
- [Service Lifetime Management](#service-lifetime-management)
- [Integration Examples](#integration-examples)
  - [ASP.NET Core](#aspnet-core)
  - [Console Applications](#console-applications)
  - [Background Services](#background-services)
- [Related Packages](#related-packages)
- [License](#license)
- [Credits](#credits)

## Overview

`Syrx.Commanders.Databases.Extensions` provides convenient extension methods for integrating Syrx database components with Microsoft's dependency injection container. This package simplifies the registration of database commanders, connectors, and related services.

## Key Features

- **Easy Registration**: Simple extension methods for service registration
- **Flexible Lifetimes**: Support for Singleton, Scoped, and Transient lifetimes
- **Fluent API**: Chainable configuration methods
- **Integration Ready**: Seamless integration with ASP.NET Core and other .NET applications
- **Convention-Based**: Follows standard .NET dependency injection patterns

## Installation

```bash
dotnet add package Syrx.Commanders.Databases.Extensions
```

**Package Manager**
```bash
Install-Package Syrx.Commanders.Databases.Extensions
```

**PackageReference**
```xml
<PackageReference Include="Syrx.Commanders.Databases.Extensions" Version="3.0.0" />
```

## Extension Methods

### Service Registration

Core extension methods for registering Syrx services:

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

Methods for fluent configuration:

```csharp
public static class ConfigurationExtensions
{
    // Configure database settings
    public static IServiceCollection ConfigureDatabaseSettings(
        this IServiceCollection services,
        Action<DatabaseSettingsOptions> configure);
}
```

## Usage

### Basic Service Registration

```csharp
using Syrx.Commanders.Databases.Extensions;

public void ConfigureServices(IServiceCollection services)
{
    // Register commanders for specific repositories
    services.AddDatabaseCommander<UserRepository>();
    services.AddDatabaseCommander<ProductRepository>();
    services.AddDatabaseCommander<OrderRepository>();
    
    // Register database connector
    services.AddDatabaseConnector<SqlServerDatabaseConnector>();
}
```

### Custom Service Lifetimes

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Singleton lifetime for long-lived services
    services.AddDatabaseCommander<CacheRepository>(ServiceLifetime.Singleton);
    
    // Scoped lifetime (default) for request-scoped operations
    services.AddDatabaseCommander<UserRepository>(ServiceLifetime.Scoped);
    
    // Transient lifetime for stateless operations
    services.AddDatabaseCommander<UtilityRepository>(ServiceLifetime.Transient);
}
```

### Advanced Configuration

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddDatabaseCommander<UserRepository>()
           .AddDatabaseCommander<ProductRepository>()
           .AddDatabaseCommander<OrderRepository>()
           .AddDatabaseConnector<SqlServerDatabaseConnector>()
           .ConfigureDatabaseSettings(options =>
           {
               options.DefaultCommandTimeout = 30;
               options.EnableRetryPolicy = true;
               options.MaxRetryAttempts = 3;
           });
}
```

## Service Lifetime Management

Understanding when to use different service lifetimes:

### Scoped (Default)
```csharp
services.AddDatabaseCommander<UserRepository>(ServiceLifetime.Scoped);
```
- **Best for**: Web applications, request-scoped operations
- **Lifetime**: One instance per HTTP request or operation scope
- **Use when**: Typical repository pattern in web applications

### Singleton
```csharp
services.AddDatabaseCommander<ConfigurationRepository>(ServiceLifetime.Singleton);
```
- **Best for**: Configuration data, cached repositories
- **Lifetime**: One instance for the entire application lifetime
- **Use when**: Data doesn't change or is cached globally

### Transient
```csharp
services.AddDatabaseCommander<UtilityRepository>(ServiceLifetime.Transient);
```
- **Best for**: Stateless operations, utility repositories
- **Lifetime**: New instance every time it's requested
- **Use when**: No shared state between operations

## Integration Examples

### ASP.NET Core

```csharp
// Program.cs or Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    // Add database services
    services.AddDatabaseCommander<UserRepository>()
           .AddDatabaseCommander<ProductRepository>()
           .AddDatabaseConnector<SqlServerDatabaseConnector>();
    
    // Add repositories themselves
    services.AddScoped<UserRepository>();
    services.AddScoped<ProductRepository>();
    
    // Configure database settings from appsettings.json
    services.Configure<DatabaseSettings>(
        Configuration.GetSection("DatabaseSettings"));
}
```

### Console Applications

```csharp
class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        
        // Register Syrx services
        services.AddDatabaseCommander<DataProcessor>()
               .AddDatabaseConnector<SqlServerDatabaseConnector>();
        
        services.AddTransient<DataProcessor>();
        
        var provider = services.BuildServiceProvider();
        
        // Use the service
        var processor = provider.GetRequiredService<DataProcessor>();
        await processor.ProcessDataAsync();
    }
}
```

### Background Services

```csharp
public class DataProcessingService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    
    public DataProcessingService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var processor = scope.ServiceProvider.GetRequiredService<DataProcessor>();
            await processor.ProcessBatchAsync();
            
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}

// Registration
services.AddDatabaseCommander<DataProcessor>(ServiceLifetime.Scoped);
services.AddScoped<DataProcessor>();
services.AddHostedService<DataProcessingService>();
```

## Related Packages

### Core Database Framework
- **[Syrx.Commanders.Databases](https://www.nuget.org/packages/Syrx.Commanders.Databases/)**: Core database command abstractions
- **[Syrx.Commanders.Databases.Connectors](https://www.nuget.org/packages/Syrx.Commanders.Databases.Connectors/)**: Database connector abstractions
- **[Syrx.Commanders.Databases.Settings](https://www.nuget.org/packages/Syrx.Commanders.Databases.Settings/)**: Configuration settings

### Database-Specific Extensions  
- **[Syrx.SqlServer.Extensions](https://www.nuget.org/packages/Syrx.SqlServer.Extensions/)**: SQL Server-specific extensions
- **[Syrx.MySql.Extensions](https://www.nuget.org/packages/Syrx.MySql.Extensions/)**: MySQL-specific extensions
- **[Syrx.Npgsql.Extensions](https://www.nuget.org/packages/Syrx.Npgsql.Extensions/)**: PostgreSQL-specific extensions
- **[Syrx.Oracle.Extensions](https://www.nuget.org/packages/Syrx.Oracle.Extensions/)**: Oracle-specific extensions

### Core Framework
- **[Syrx](https://www.nuget.org/packages/Syrx/)**: Core interfaces and abstractions

## License

This project is licensed under the [MIT License](https://github.com/Syrx/Syrx/blob/main/LICENSE).

## Credits

Built on top of [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection/) and [Dapper](https://github.com/DapperLib/Dapper).
````



