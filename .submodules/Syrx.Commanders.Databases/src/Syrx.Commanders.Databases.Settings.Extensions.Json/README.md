# Syrx.Commanders.Databases.Settings.Extensions.Json

JSON configuration file support for Syrx database settings.

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Installation](#installation)
- [Extension Methods](#extension-methods)
- [Usage](#usage)
  - [Basic JSON Configuration](#basic-json-configuration)
  - [Service Registration](#service-registration)
  - [Configuration File Loading](#configuration-file-loading)
- [JSON Schema](#json-schema)
- [Configuration Examples](#configuration-examples)
  - [Simple Configuration](#simple-configuration)
  - [Multi-Database Configuration](#multi-database-configuration)
  - [Complex Command Configuration](#complex-command-configuration)
- [File Management](#file-management)
  - [Environment-Specific Files](#environment-specific-files)
  - [Configuration Validation](#configuration-validation)
- [Integration Examples](#integration-examples)
  - [ASP.NET Core](#aspnet-core)
  - [Console Application](#console-application)
- [Related Packages](#related-packages)
- [License](#license)
- [Credits](#credits)

## Overview

`Syrx.Commanders.Databases.Settings.Extensions.Json` provides JSON file configuration support for Syrx database settings. This package enables you to define your database commands, connection strings, and configuration in JSON files that can be loaded at runtime.



## Key Features

- **JSON Configuration**: Define database settings in JSON format
- **File Loading**: Load configuration from JSON files
- **Service Integration**: Seamless integration with dependency injection
- **Environment Support**: Support for environment-specific configuration files
- **Validation**: JSON schema validation for configuration integrity
- **Hot Reload**: Support for configuration reloading (when supported by host)

## Installation

```bash
dotnet add package Syrx.Commanders.Databases.Settings.Extensions.Json
```

**Package Manager**
```bash
Install-Package Syrx.Commanders.Databases.Settings.Extensions.Json
```

**PackageReference**
```xml
<PackageReference Include="Syrx.Commanders.Databases.Settings.Extensions.Json" Version="3.0.0" />
```

> **Note**: The `Syrx.Extensions` package (which provides the `UseSyrx()` method) is automatically included as a dependency.

## Extension Methods

Key extension methods for JSON configuration:

```csharp
public static class UseFileExtensions
{
    // Add JSON file to SyrxBuilder configuration
    public static SyrxBuilder UseFile(
        this SyrxBuilder factory, 
        string fileName, 
        IConfigurationBuilder builder);
}
```

> **Note**: This package extends the main `UseSyrx()` pattern from the `Syrx.Extensions` package, which is automatically included as a dependency.

## Usage

### Basic JSON Configuration

Create a JSON configuration file (`syrx.json`):

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
              "ConnectionAlias": "DefaultConnection",
              "CommandTimeout": 30
            },
            "RetrieveAsync": {
              "CommandText": "SELECT * FROM Users",
              "ConnectionAlias": "DefaultConnection"
            }
          }
        }
      ]
    }
  ]
}
```

### Service Registration

Register JSON configuration using the Syrx builder pattern:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    var configBuilder = new ConfigurationBuilder();
    
    // Single JSON file
    services.UseSyrx(builder => builder
        .UseFile("syrx.json", configBuilder));
    
    // Multiple JSON files (later files override earlier ones)
    services.UseSyrx(builder => builder
        .UseFile("syrx.json", configBuilder)
        .UseFile("syrx.production.json", configBuilder));
}
```

### Configuration File Loading

```csharp
// Using with dependency injection
public void ConfigureServices(IServiceCollection services)
{
    var configBuilder = new ConfigurationBuilder();
    
    services.UseSyrx(builder => builder
        .UseFile("syrx.json", configBuilder));
    
    // Build and use the configuration
    var configuration = configBuilder.Build();
}
```

## JSON Schema

The JSON configuration follows this structure:

```json
{
  "$schema": "https://schemas.syrx.dev/commander-settings.json",
  "Connections": [
    {
      "Alias": "string",
      "ConnectionString": "string"
    }
  ],
  "Namespaces": [
    {
      "Name": "string",
      "Types": [
        {
          "Name": "string", 
          "Commands": {
            "methodName": {
              "CommandText": "string",
              "ConnectionAlias": "string",
              "CommandTimeout": "integer (default: 30)",
              "CommandType": "Text|StoredProcedure|TableDirect (default: Text)",
              "SplitOn": "string (optional)",
              "IsolationLevel": "ReadCommitted|ReadUncommitted|RepeatableRead|Serializable (optional)"
            }
          }
        }
      ]
    }
  ]
}
```

## Configuration Examples

### Simple Configuration

```json
{
  "Connections": [
    {
      "Alias": "Database",
      "ConnectionString": "Server=localhost;Database=SimpleApp;Trusted_Connection=true;"
    }
  ],
  "Namespaces": [
    {
      "Name": "SimpleApp.Repositories",
      "Types": [
        {
          "Name": "UserRepository",
          "Commands": {
            "RetrieveAllUsersAsync": {
              "CommandText": "SELECT Id, Name, Email FROM Users",
              "ConnectionAlias": "Database"
            },
            "RetrieveUserByIdAsync": {
              "CommandText": "SELECT Id, Name, Email FROM Users WHERE Id = @id",
              "ConnectionAlias": "Database",
              "CommandTimeout": 30
            },
            "CreateUserAsync": {
              "CommandText": "INSERT INTO Users (Name, Email) VALUES (@Name, @Email)",
              "ConnectionAlias": "Database"
            }
          }
        }
      ]
    }
  ]
}
```

### Multi-Database Configuration

```json
{
  "Connections": [
    {
      "Alias": "UserDatabase",
      "ConnectionString": "Server=user-db;Database=Users;Trusted_Connection=true;"
    },
    {
      "Alias": "ProductDatabase", 
      "ConnectionString": "Server=product-db;Database=Products;Trusted_Connection=true;"
    },
    {
      "Alias": "OrderDatabase",
      "ConnectionString": "Server=order-db;Database=Orders;Trusted_Connection=true;"
    }
  ],
  "Namespaces": [
    {
      "Name": "MyApp.Repositories",
      "Types": [
        {
          "Name": "UserRepository",
          "Commands": {
            "RetrieveUsersAsync": {
              "CommandText": "SELECT * FROM Users",
              "ConnectionAlias": "UserDatabase"
            }
          }
        },
        {
          "Name": "ProductRepository", 
          "Commands": {
            "RetrieveProductsAsync": {
              "CommandText": "SELECT * FROM Products",
              "ConnectionAlias": "ProductDatabase"
            }
          }
        },
        {
          "Name": "OrderRepository",
          "Commands": {
            "RetrieveOrdersAsync": {
              "CommandText": "SELECT * FROM Orders",
              "ConnectionAlias": "OrderDatabase"
            }
          }
        }
      ]
    }
  ]
}
```

### Complex Command Configuration

```json
{
  "Connections": [
    {
      "Alias": "Primary",
      "ConnectionString": "Server=primary;Database=MyApp;Trusted_Connection=true;"
    },
    {
      "Alias": "ReadOnly",
      "ConnectionString": "Server=readonly;Database=MyApp;Trusted_Connection=true;"
    }
  ],
  "Namespaces": [
    {
      "Name": "MyApp.Repositories",
      "Types": [
        {
          "Name": "OrderRepository",
          "Commands": {
            "RetrieveOrdersWithDetailsAsync": {
              "CommandText": "SELECT o.*, c.*, oi.*, p.* FROM Orders o JOIN Customers c ON o.CustomerId = c.Id JOIN OrderItems oi ON o.Id = oi.OrderId JOIN Products p ON oi.ProductId = p.Id WHERE o.OrderDate >= @fromDate",
              "ConnectionAlias": "ReadOnly",
              "CommandTimeout": 60,
              "SplitOn": "Id,Id,Id"
            },
            "ProcessOrderAsync": {
              "CommandText": "sp_ProcessOrder",
              "ConnectionAlias": "Primary",
              "CommandType": "StoredProcedure",
              "CommandTimeout": 300,
              "IsolationLevel": "Serializable"
            },
            "RetrieveOrderStatisticsAsync": {
              "CommandText": "SELECT COUNT(*) as TotalOrders, SUM(Total) as TotalAmount FROM Orders WHERE OrderDate >= @fromDate",
              "ConnectionAlias": "ReadOnly",
              "CommandTimeout": 45
            }
          }
        }
      ]
    }
  ]
}
```

## File Management

### Environment-Specific Files

Support for environment-specific configuration:

```bash
# Base configuration
syrx.json

# Environment-specific overrides
syrx.Development.json
syrx.Staging.json  
syrx.Production.json
```

```csharp
public void ConfigureServices(IServiceCollection services)
{
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    var configBuilder = new ConfigurationBuilder();
    
    // Load base configuration first, then environment-specific overrides
    var environmentFile = $"syrx.{environment}.json";
    
    services.UseSyrx(builder => {
        var syrxBuilder = builder.UseFile("syrx.json", configBuilder);
        
        if (File.Exists(environmentFile))
        {
            syrxBuilder.UseFile(environmentFile, configBuilder);
        }
        
        return syrxBuilder;
    });
}
```

### Configuration Validation

Enable JSON schema validation:

```csharp
var configBuilder = new ConfigurationBuilder();

// Direct builder usage (less common)
var services = new ServiceCollection();
services.UseSyrx(builder => builder
    .UseFile("syrx.json", configBuilder));

// Build configuration
var configuration = configBuilder.Build();
```

## Repository Implementation

Your repositories should use `ICommander<TRepository>` dependency injection:

```csharp
public class UserRepository
{
    private readonly ICommander<UserRepository> _commander;
    
    public UserRepository(ICommander<UserRepository> commander)
    {
        _commander = commander;
    }
    
    // Method names automatically map to JSON configuration commands
    public async Task<IEnumerable<User>> RetrieveAllUsersAsync(CancellationToken cancellationToken = default)
    {
      return await _commander.QueryAsync<User>(cancellationToken: cancellationToken);
    }
    
    public async Task<User> RetrieveUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
      var result = await _commander.QueryAsync<User>(new { id }, cancellationToken);
        return result.FirstOrDefault();
    }
    
    public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
      return await _commander.ExecuteAsync(user, cancellationToken) ? user : default;
    }
    
    public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
      return await _commander.ExecuteAsync(user, cancellationToken) ? user : default;
    }
    
    public async Task<User> DeleteUserAsync(User user, CancellationToken cancellationToken = default)
    {
      return await _commander.ExecuteAsync(user, cancellationToken) ? user : default;
    }
}
```

> **Important**: Method names like `RetrieveAllUsersAsync` automatically map to command configurations in your JSON file via the pattern: `{Namespace}.{ClassName}.{MethodName}`

## Integration Examples

### ASP.NET Core

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add Syrx JSON configuration
var configBuilder = new ConfigurationBuilder();
builder.Services.UseSyrx(syrxBuilder => syrxBuilder
    .UseFile("syrx.json", configBuilder));

// Add other services
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ProductRepository>();

var app = builder.Build();
```

```csharp
// Alternative with IConfiguration integration
public void ConfigureServices(IServiceCollection services)
{
    var configBuilder = new ConfigurationBuilder();
    var configFile = Configuration.GetValue<string>("SyrxConfigurationFile");
    
    services.UseSyrx(builder => builder
        .UseFile(configFile, configBuilder));
}
```

### Console Application

```csharp
class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        var configBuilder = new ConfigurationBuilder();
        
        // Load JSON configuration
        services.UseSyrx(builder => builder
            .UseFile("syrx.json", configBuilder));
        
        // Register repositories
        services.AddScoped<UserRepository>();
        
        var provider = services.BuildServiceProvider();
        
        // Use the configured repository
        var userRepository = provider.GetRequiredService<UserRepository>();
        var users = await userRepository.RetrieveAllUsersAsync();
        
        foreach (var user in users)
        {
            Console.WriteLine($"User: {user.Name} ({user.Email})");
        }
    }
}
```

## Related Packages

### Configuration Extensions
- **[Syrx.Commanders.Databases.Settings.Extensions](https://www.nuget.org/packages/Syrx.Commanders.Databases.Settings.Extensions/)**: Builder pattern extensions
- **[Syrx.Commanders.Databases.Settings.Extensions.Xml](https://www.nuget.org/packages/Syrx.Commanders.Databases.Settings.Extensions.Xml/)**: XML configuration support

### Configuration Readers
- **[Syrx.Commanders.Databases.Settings.Readers](https://www.nuget.org/packages/Syrx.Commanders.Databases.Settings.Readers/)**: Configuration file readers
- **[Syrx.Commanders.Databases.Settings.Readers.Extensions](https://www.nuget.org/packages/Syrx.Commanders.Databases.Settings.Readers.Extensions/)**: Reader extensions

### Core Framework
- **[Syrx.Commanders.Databases.Settings](https://www.nuget.org/packages/Syrx.Commanders.Databases.Settings/)**: Core settings classes
- **[Syrx.Commanders.Databases](https://www.nuget.org/packages/Syrx.Commanders.Databases/)**: Database command abstractions

## License

This project is licensed under the [MIT License](https://github.com/Syrx/Syrx/blob/main/LICENSE).

## Credits

Built on top of [System.Text.Json](https://www.nuget.org/packages/System.Text.Json/) and [Dapper](https://github.com/DapperLib/Dapper).



