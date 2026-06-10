````markdown
# Syrx.Commanders.Databases.Settings.Extensions

Builder pattern extensions and fluent APIs for configuring Syrx database settings.

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Installation](#installation)
- [Builder Classes](#builder-classes)
  - [CommanderSettingsBuilder](#commandersettingsbuilder)
  - [CommandSettingBuilder](#commandsettingbuilder)  
  - [ConnectionStringSettingsBuilder](#connectionstringsettingsbuilder)
  - [NamespaceSettingBuilder](#namespacesettingbuilder)
  - [TypeSettingBuilder](#typesettingbuilder)
- [Usage](#usage)
  - [Basic Configuration](#basic-configuration)
  - [Fluent Configuration](#fluent-configuration)
  - [Advanced Configuration](#advanced-configuration)
- [Builder Methods](#builder-methods)
  - [Connection String Configuration](#connection-string-configuration)
  - [Command Configuration](#command-configuration)
  - [Namespace and Type Configuration](#namespace-and-type-configuration)
- [Configuration Examples](#configuration-examples)
  - [Simple Repository Setup](#simple-repository-setup)
  - [Multi-Database Setup](#multi-database-setup)
  - [Complex Command Configuration](#complex-command-configuration)
- [Related Packages](#related-packages)
- [License](#license)
- [Credits](#credits)

## Overview

`Syrx.Commanders.Databases.Settings.Extensions` provides fluent builder APIs for constructing Syrx database configuration settings. This package makes it easy to build complex database configurations using a readable, chainable syntax.

## Key Features

- **Fluent API**: Chainable builder methods for readable configuration
- **Type Safety**: Strongly-typed builder classes with validation
- **Incremental Building**: Build configurations step-by-step
- **Validation**: Built-in validation during configuration building
- **Extensibility**: Easy to extend with custom configuration options

## Installation

```bash
dotnet add package Syrx.Commanders.Databases.Settings.Extensions
```

**Package Manager**
```bash
Install-Package Syrx.Commanders.Databases.Settings.Extensions
```

**PackageReference**
```xml
<PackageReference Include="Syrx.Commanders.Databases.Settings.Extensions" Version="3.0.0" />
```

## Builder Classes

### CommanderSettingsBuilder

The root builder for creating complete commander settings. **Thread-safe** - can be used safely across multiple threads.

```csharp
public class CommanderSettingsBuilder
{
    public CommanderSettingsBuilder AddConnectionString(string alias, string connectionString);
    public CommanderSettingsBuilder AddNamespace(string name, Action<NamespaceSettingBuilder> configure);
    public CommanderSettings Build();
}
```

### CommandSettingBuilder

Builder for individual command settings:

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

### ConnectionStringSettingsBuilder

Builder for connection string configuration:

```csharp
public class ConnectionStringSettingsBuilder
{
    public ConnectionStringSettingsBuilder Add(string alias, string connectionString);
    public IEnumerable<ConnectionStringSetting> Build();
}
```

### NamespaceSettingBuilder

Builder for namespace-level configuration. **Thread-safe** - can be used safely across multiple threads.

```csharp
public class NamespaceSettingBuilder
{
    public NamespaceSettingBuilder AddType(string typeName, Action<TypeSettingBuilder> configure);
    public NamespaceSetting Build();
}
```

### TypeSettingBuilder

Builder for type-level configuration. **Thread-safe** - can be used safely across multiple threads.

```csharp
public class TypeSettingBuilder
{
    public TypeSettingBuilder AddCommand(string methodName, Action<CommandSettingBuilder> configure);
    public TypeSetting Build();
}
```

## Usage

### Basic Configuration

```csharp
using Syrx.Commanders.Databases.Settings.Extensions;

var settings = new CommanderSettingsBuilder()
    .AddConnectionString("Default", "Server=localhost;Database=MyApp;Trusted_Connection=true;")
    .AddNamespace("MyApp.Repositories", ns => ns
        .AddType("UserRepository", type => type
            .AddCommand("RetrieveAsync", cmd => cmd
                .UseCommandText("SELECT * FROM Users WHERE Id = @id")
                .UseConnectionAlias("Default")
                .SetCommandTimeout(30))))
    .Build();
```

### Fluent Configuration

```csharp
var builder = new CommanderSettingsBuilder();

// Add multiple connection strings
builder.AddConnectionString("Primary", primaryConnectionString)
       .AddConnectionString("ReadOnly", readOnlyConnectionString)
       .AddConnectionString("Analytics", analyticsConnectionString);

// Configure multiple namespaces
builder.AddNamespace("MyApp.Repositories.User", ns => ns
    .AddType("UserRepository", ConfigureUserRepository)
    .AddType("UserProfileRepository", ConfigureUserProfileRepository))
   .AddNamespace("MyApp.Repositories.Product", ns => ns
    .AddType("ProductRepository", ConfigureProductRepository)
    .AddType("CategoryRepository", ConfigureCategoryRepository));

var settings = builder.Build();
```

### Advanced Configuration

```csharp
var settings = new CommanderSettingsBuilder()
    .AddConnectionString("Master", masterConnectionString)
    .AddConnectionString("Slave", slaveConnectionString)
    .AddNamespace("MyApp.Repositories", ns => ns
        .AddType("UserRepository", type => type
            // Read operations use slave
            .AddCommand("RetrieveAllUsersAsync", cmd => cmd
                .UseCommandText("SELECT * FROM Users WHERE IsActive = 1")
                .UseConnectionAlias("Slave")
                .SetCommandTimeout(30))
            
            // Write operations use master
            .AddCommand("CreateUserAsync", cmd => cmd
                .UseCommandText("INSERT INTO Users (Name, Email, CreatedDate) VALUES (@Name, @Email, @CreatedDate)")
                .UseConnectionAlias("Master")
                .SetCommandTimeout(60)
                .SetIsolationLevel(IsolationLevel.ReadCommitted))
            
            // Complex queries with multi-mapping
            .AddCommand("RetrieveWithProfilesAsync", cmd => cmd
                .UseCommandText(@"
                    SELECT u.*, p.*
                    FROM Users u
                    JOIN UserProfiles p ON u.Id = p.UserId
                    WHERE u.IsActive = 1")
                .UseConnectionAlias("Slave")
                .SplitOn("Id")
                .SetCommandTimeout(45))))
    .Build();
```

## Builder Methods

### Connection String Configuration

```csharp
// Single connection string
builder.AddConnectionString("Default", connectionString);

// Multiple connection strings
builder.AddConnectionString("Primary", primaryConnection)
       .AddConnectionString("ReadOnly", readOnlyConnection)
       .AddConnectionString("Cache", cacheConnection);
```

### Command Configuration

```csharp
type.AddCommand("RetrieveUserByIdAsync", cmd => cmd
    .UseCommandText("SELECT * FROM Users WHERE Id = @id")
    .UseConnectionAlias("Primary")
    .SetCommandTimeout(30)
    .SetCommandType(CommandType.Text)
    .SetIsolationLevel(IsolationLevel.ReadCommitted));

// Stored procedure configuration
type.AddCommand("RetrieveUserStatistics", cmd => cmd
    .UseCommandText("sp_GetUserStats")
    .UseConnectionAlias("Analytics")
    .SetCommandType(CommandType.StoredProcedure)
    .SetCommandTimeout(120));
```

### Namespace and Type Configuration

```csharp
builder.AddNamespace("MyApp.Data.Repositories", ns => ns
    .AddType("UserRepository", ConfigureUserRepository)
    .AddType("ProductRepository", ConfigureProductRepository)
    .AddType("OrderRepository", ConfigureOrderRepository));

void ConfigureUserRepository(TypeSettingBuilder type)
{
    type.AddCommand("RetrieveAsync", cmd => cmd
        .UseCommandText("SELECT * FROM Users WHERE Id = @id")
        .UseConnectionAlias("Default"));
        
    type.AddCommand("CreateAsync", cmd => cmd
        .UseCommandText("INSERT INTO Users (...) VALUES (...)")
        .UseConnectionAlias("Default"));
}
```

## Configuration Examples

### Simple Repository Setup

```csharp
public CommanderSettings BuildSimpleConfiguration()
{
    return new CommanderSettingsBuilder()
        .AddConnectionString("Database", connectionString)
        .AddNamespace("MyApp.Repositories", ns => ns
            .AddType("UserRepository", type => type
                .AddCommand("RetrieveAsync", cmd => cmd
                    .UseCommandText("SELECT * FROM Users")
                    .UseConnectionAlias("Database"))
                .AddCommand("RetrieveAsync", cmd => cmd
                    .UseCommandText("SELECT * FROM Users WHERE Id = @id")
                    .UseConnectionAlias("Database"))
                .AddCommand("CreateAsync", cmd => cmd
                    .UseCommandText("INSERT INTO Users (Name, Email) VALUES (@Name, @Email)")
                    .UseConnectionAlias("Database"))))
        .Build();
}
```

### Multi-Database Setup

```csharp
public CommanderSettings BuildMultiDatabaseConfiguration()
{
    return new CommanderSettingsBuilder()
        .AddConnectionString("UserDB", userDbConnectionString)
        .AddConnectionString("ProductDB", productDbConnectionString)
        .AddConnectionString("OrderDB", orderDbConnectionString)
        .AddNamespace("MyApp.Repositories", ns => ns
            .AddType("UserRepository", type => type
                .AddCommand("RetrieveUsersAsync", cmd => cmd
                    .UseCommandText("SELECT * FROM Users")
                    .UseConnectionAlias("UserDB")))
            .AddType("ProductRepository", type => type
                .AddCommand("RetrieveProductsAsync", cmd => cmd
                    .UseCommandText("SELECT * FROM Products")
                    .UseConnectionAlias("ProductDB")))
            .AddType("OrderRepository", type => type
                .AddCommand("RetrieveOrdersAsync", cmd => cmd
                    .UseCommandText("SELECT * FROM Orders")
                    .UseConnectionAlias("OrderDB"))))
        .Build();
}
```

### Complex Command Configuration

```csharp
public void ConfigureComplexCommands(TypeSettingBuilder type)
{
    // Complex query with multiple joins
    type.AddCommand("RetrieveOrdersWithDetailsAsync", cmd => cmd
        .UseCommandText(@"
            SELECT o.*, c.*, oi.*, p.*
            FROM Orders o
            JOIN Customers c ON o.CustomerId = c.Id
            JOIN OrderItems oi ON o.Id = oi.OrderId
            JOIN Products p ON oi.ProductId = p.Id
            WHERE o.OrderDate >= @fromDate")
        .UseConnectionAlias("Primary")
        .SplitOn("Id,Id,Id")
        .SetCommandTimeout(60));

    // Stored procedure with output parameters
    type.AddCommand("ProcessOrderAsync", cmd => cmd
        .UseCommandText("sp_ProcessOrder")
        .UseConnectionAlias("Primary")
        .SetCommandType(CommandType.StoredProcedure)
        .SetCommandTimeout(300)
        .SetIsolationLevel(IsolationLevel.Serializable));

    // Bulk insert operation
    type.AddCommand("BulkInsertOrdersAsync", cmd => cmd
        .UseCommandText("INSERT INTO Orders (CustomerId, OrderDate, Total) VALUES (@CustomerId, @OrderDate, @Total)")
        .UseConnectionAlias("Primary")
        .SetCommandTimeout(600));
}
```

## Related Packages

### Core Settings
- **[Syrx.Commanders.Databases.Settings](https://www.nuget.org/packages/Syrx.Commanders.Databases.Settings/)**: Core settings classes and models

### Configuration Support
- **[Syrx.Commanders.Databases.Settings.Extensions.Json](https://www.nuget.org/packages/Syrx.Commanders.Databases.Settings.Extensions.Json/)**: JSON configuration file support
- **[Syrx.Commanders.Databases.Settings.Extensions.Xml](https://www.nuget.org/packages/Syrx.Commanders.Databases.Settings.Extensions.Xml/)**: XML configuration file support
- **[Syrx.Commanders.Databases.Settings.Readers](https://www.nuget.org/packages/Syrx.Commanders.Databases.Settings.Readers/)**: Configuration file readers

### Core Framework
- **[Syrx.Commanders.Databases](https://www.nuget.org/packages/Syrx.Commanders.Databases/)**: Database command abstractions
- **[Syrx](https://www.nuget.org/packages/Syrx/)**: Core interfaces

## License

This project is licensed under the [MIT License](https://github.com/Syrx/Syrx/blob/main/LICENSE).

## Credits

Built on top of [Dapper](https://github.com/DapperLib/Dapper) and standard .NET configuration patterns.
````



