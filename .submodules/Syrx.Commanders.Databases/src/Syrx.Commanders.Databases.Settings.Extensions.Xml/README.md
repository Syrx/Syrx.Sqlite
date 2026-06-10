# Syrx.Commanders.Databases.Settings.Extensions.Xml

XML configuration file support for Syrx database settings.

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Installation](#installation)
- [Extension Methods](#extension-methods)
- [Usage](#usage)
  - [Basic XML Configuration](#basic-xml-configuration)
  - [Service Registration](#service-registration)
  - [Configuration File Loading](#configuration-file-loading)
- [XML Schema](#xml-schema)
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

`Syrx.Commanders.Databases.Settings.Extensions.Xml` provides XML file configuration support for Syrx database settings. This package enables you to define your database commands, connection strings, and configuration in XML files that can be loaded at runtime.



## Key Features

- **XML Configuration**: Define database settings in XML format
- **File Loading**: Load configuration from XML files
- **Service Integration**: Seamless integration with dependency injection
- **Environment Support**: Support for environment-specific configuration files
- **Schema Validation**: XML schema validation for configuration integrity
- **Namespace Support**: Proper XML namespace handling

## Installation

```bash
dotnet add package Syrx.Commanders.Databases.Settings.Extensions.Xml
```

**Package Manager**
```bash
Install-Package Syrx.Commanders.Databases.Settings.Extensions.Xml
```

**PackageReference**
```xml
<PackageReference Include="Syrx.Commanders.Databases.Settings.Extensions.Xml" Version="3.0.0" />
```

> **Note**: The `Syrx.Extensions` package (which provides the `UseSyrx()` method) is automatically included as a dependency.

## Extension Methods

Key extension methods for XML configuration:

```csharp
public static class UseFileExtensions
{
    // Add XML file to SyrxBuilder configuration
    public static SyrxBuilder UseFile(
        this SyrxBuilder factory, 
        string fileName, 
        IConfigurationBuilder builder);
}
```

> **Note**: This package extends the main `UseSyrx()` pattern from the `Syrx.Extensions` package, which is automatically included as a dependency.

## Usage

### Basic XML Configuration

Create an XML configuration file (`syrx.xml`):

```xml
<?xml version="1.0" encoding="utf-8"?>
<CommanderSettings xmlns="http://schemas.syrx.dev/commander-settings">
  <Connections>
    <ConnectionStringSetting>
      <Alias>DefaultConnection</Alias>
      <ConnectionString>Server=localhost;Database=MyApp;Trusted_Connection=true;</ConnectionString>
    </ConnectionStringSetting>
  </Connections>
  <Namespaces>
    <NamespaceSetting>
      <Name>MyApp.Repositories</Name>
      <Types>
        <TypeSetting>
          <Name>UserRepository</Name>
          <Commands>
            <Command Key="RetrieveAsync">
              <CommandText>SELECT * FROM Users WHERE Id = @id</CommandText>
              <ConnectionAlias>DefaultConnection</ConnectionAlias>
              <CommandTimeout>30</CommandTimeout>
            </Command>
            <Command Key="RetrieveAsync">
              <CommandText>SELECT * FROM Users</CommandText>
              <ConnectionAlias>DefaultConnection</ConnectionAlias>
            </Command>
          </Commands>
        </TypeSetting>
      </Types>
    </NamespaceSetting>
  </Namespaces>
</CommanderSettings>
```

### Service Registration

Register XML configuration in your application:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    var builder = new ConfigurationBuilder();
    
    // Single XML file
    services.UseSyrx(builder => builder
        .UseFile("syrx.xml", configBuilder));
    
    // Multiple XML files (later files override earlier ones)  
    services.UseSyrx(builder => builder
        .UseFile("syrx.xml", configBuilder)
        .UseFile("syrx.production.xml", configBuilder));
}
```

### Configuration File Loading

```csharp
// Using with dependency injection
public void ConfigureServices(IServiceCollection services)
{
    var configBuilder = new ConfigurationBuilder();
    
    services.UseSyrx(builder => builder
        .UseFile("syrx.xml", configBuilder));
    
    // Build and use the configuration
    var configuration = configBuilder.Build();
}
```

## XML Schema

The XML configuration follows this structure with proper namespace support:

```xml
<?xml version="1.0" encoding="utf-8"?>
<xs:schema 
    targetNamespace="http://schemas.syrx.dev/commander-settings"
    xmlns="http://schemas.syrx.dev/commander-settings"
    xmlns:xs="http://www.w3.org/2001/XMLSchema"
    elementFormDefault="qualified">
  
  <xs:element name="CommanderSettings">
    <xs:complexType>
      <xs:sequence>
        <xs:element name="Connections" type="ConnectionStringsType" minOccurs="0"/>
        <xs:element name="Namespaces" type="NamespacesType" minOccurs="0"/>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
  
  <!-- Additional schema definitions... -->
</xs:schema>
```

## Configuration Examples

### Simple Configuration

```xml
<?xml version="1.0" encoding="utf-8"?>
<CommanderSettings xmlns="http://schemas.syrx.dev/commander-settings">
  <Connections>
    <ConnectionStringSetting>
      <Alias>Database</Alias>
      <ConnectionString>Server=localhost;Database=SimpleApp;Trusted_Connection=true;</ConnectionString>
    </ConnectionStringSetting>
  </Connections>
  <Namespaces>
    <NamespaceSetting>
      <Name>SimpleApp.Repositories</Name>
      <Types>
        <TypeSetting>
          <Name>UserRepository</Name>
          <Commands>
            <Command Key="RetrieveAllUsersAsync">
              <CommandText>SELECT Id, Name, Email FROM Users</CommandText>
              <ConnectionAlias>Database</ConnectionAlias>
            </Command>
            <Command Key="RetrieveUserByIdAsync">
              <CommandText>SELECT Id, Name, Email FROM Users WHERE Id = @id</CommandText>
              <ConnectionAlias>Database</ConnectionAlias>
              <CommandTimeout>30</CommandTimeout>
            </Command>
            <Command Key="CreateUserAsync">
              <CommandText>INSERT INTO Users (Name, Email) VALUES (@Name, @Email)</CommandText>
              <ConnectionAlias>Database</ConnectionAlias>
            </Command>
          </Commands>
        </TypeSetting>
      </Types>
    </NamespaceSetting>
  </Namespaces>
</CommanderSettings>
```

### Multi-Database Configuration

```xml
<?xml version="1.0" encoding="utf-8"?>
<CommanderSettings xmlns="http://schemas.syrx.dev/commander-settings">
  <Connections>
    <ConnectionStringSetting>
      <Alias>UserDatabase</Alias>
      <ConnectionString>Server=user-db;Database=Users;Trusted_Connection=true;</ConnectionString>
    </ConnectionStringSetting>
    <ConnectionStringSetting>
      <Alias>ProductDatabase</Alias>
      <ConnectionString>Server=product-db;Database=Products;Trusted_Connection=true;</ConnectionString>
    </ConnectionStringSetting>
    <ConnectionStringSetting>
      <Alias>OrderDatabase</Alias>
      <ConnectionString>Server=order-db;Database=Orders;Trusted_Connection=true;</ConnectionString>
    </ConnectionStringSetting>
  </Connections>
  <Namespaces>
    <NamespaceSetting>
      <Name>MyApp.Repositories</Name>
      <Types>
        <TypeSetting>
          <Name>UserRepository</Name>
          <Commands>
            <Command Key="RetrieveUsersAsync">
              <CommandText>SELECT * FROM Users</CommandText>
              <ConnectionAlias>UserDatabase</ConnectionAlias>
            </Command>
          </Commands>
        </TypeSetting>
        <TypeSetting>
          <Name>ProductRepository</Name>
          <Commands>
            <Command Key="RetrieveProductsAsync">
              <CommandText>SELECT * FROM Products</CommandText>
              <ConnectionAlias>ProductDatabase</ConnectionAlias>
            </Command>
          </Commands>
        </TypeSetting>
        <TypeSetting>
          <Name>OrderRepository</Name>
          <Commands>
            <Command Key="RetrieveOrdersAsync">
              <CommandText>SELECT * FROM Orders</CommandText>
              <ConnectionAlias>OrderDatabase</ConnectionAlias>
            </Command>
          </Commands>
        </TypeSetting>
      </Types>
    </NamespaceSetting>
  </Namespaces>
</CommanderSettings>
```

### Complex Command Configuration

```xml
<?xml version="1.0" encoding="utf-8"?>
<CommanderSettings xmlns="http://schemas.syrx.dev/commander-settings">
  <Connections>
    <ConnectionStringSetting>
      <Alias>Primary</Alias>
      <ConnectionString>Server=primary;Database=MyApp;Trusted_Connection=true;</ConnectionString>
    </ConnectionStringSetting>
    <ConnectionStringSetting>
      <Alias>ReadOnly</Alias>
      <ConnectionString>Server=readonly;Database=MyApp;Trusted_Connection=true;</ConnectionString>
    </ConnectionStringSetting>
  </Connections>
  <Namespaces>
    <NamespaceSetting>
      <Name>MyApp.Repositories</Name>
      <Types>
        <TypeSetting>
          <Name>OrderRepository</Name>
          <Commands>
            <Command Key="RetrieveOrdersWithDetailsAsync">
              <CommandText><![CDATA[
                SELECT o.*, c.*, oi.*, p.*
                FROM Orders o
                JOIN Customers c ON o.CustomerId = c.Id
                JOIN OrderItems oi ON o.Id = oi.OrderId
                JOIN Products p ON oi.ProductId = p.Id
                WHERE o.OrderDate >= @fromDate
              ]]></CommandText>
              <ConnectionAlias>ReadOnly</ConnectionAlias>
              <CommandTimeout>60</CommandTimeout>
              <SplitOn>Id,Id,Id</SplitOn>
            </Command>
            <Command Key="ProcessOrderAsync">
              <CommandText>sp_ProcessOrder</CommandText>
              <ConnectionAlias>Primary</ConnectionAlias>
              <CommandType>StoredProcedure</CommandType>
              <CommandTimeout>300</CommandTimeout>
              <IsolationLevel>Serializable</IsolationLevel>
            </Command>
            <Command Key="RetrieveOrderStatisticsAsync">
              <CommandText><![CDATA[
                SELECT COUNT(*) as TotalOrders, SUM(Total) as TotalAmount
                FROM Orders
                WHERE OrderDate >= @fromDate
              ]]></CommandText>
              <ConnectionAlias>ReadOnly</ConnectionAlias>
              <CommandTimeout>45</CommandTimeout>
            </Command>
          </Commands>
        </TypeSetting>
      </Types>
    </NamespaceSetting>
  </Namespaces>
</CommanderSettings>
```

## File Management

### Environment-Specific Files

Support for environment-specific configuration:

```bash
# Base configuration
syrx.xml

# Environment-specific overrides
syrx.Development.xml
syrx.Staging.xml  
syrx.Production.xml
```

```csharp
public void ConfigureServices(IServiceCollection services)
{
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
    var configBuilder = new ConfigurationBuilder();
    
    // Load base configuration first, then environment-specific overrides
    var environmentFile = $"syrx.{environment}.xml";
    
    services.UseSyrx(builder => {
        var syrxBuilder = builder.UseFile("syrx.xml", configBuilder);
        
        if (File.Exists(environmentFile))
        {
            syrxBuilder.UseFile(environmentFile, configBuilder);
        }
        
        return syrxBuilder;
    });
}
```

### Configuration Validation

Enable XML schema validation:

```csharp
var builder = new ConfigurationBuilder();
var syrxBuilder = new SyrxBuilder();

// Add XML file to configuration
var settings = syrxBuilder.UseFile("syrx.xml", builder);
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
    
    // Method names automatically map to XML configuration commands
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
    
    public async Task<User> DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
      return await _commander.ExecuteAsync(new { id }, cancellationToken) ? new User { Id = id } : default;
    }
}
```

> **Important**: Method names like `RetrieveAllUsersAsync` automatically map to command configurations in your XML file via the pattern: `{Namespace}.{ClassName}.{MethodName}`

## Integration Examples

### ASP.NET Core

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add Syrx XML configuration
var configBuilder = new ConfigurationBuilder();
builder.Services.UseSyrx(syrxBuilder => syrxBuilder
    .UseFile("syrx.xml", configBuilder));

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
        
        // Load XML configuration
        services.UseSyrx(builder => builder
            .UseFile("syrx.xml", configBuilder));
        
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
- **[Syrx.Commanders.Databases.Settings.Extensions.Json](https://www.nuget.org/packages/Syrx.Commanders.Databases.Settings.Extensions.Json/)**: JSON configuration support

### Configuration Readers
- **[Syrx.Commanders.Databases.Settings.Readers](https://www.nuget.org/packages/Syrx.Commanders.Databases.Settings.Readers/)**: Configuration file readers
- **[Syrx.Commanders.Databases.Settings.Readers.Extensions](https://www.nuget.org/packages/Syrx.Commanders.Databases.Settings.Readers.Extensions/)**: Reader extensions

### Core Framework
- **[Syrx.Commanders.Databases.Settings](https://www.nuget.org/packages/Syrx.Commanders.Databases.Settings/)**: Core settings classes
- **[Syrx.Commanders.Databases](https://www.nuget.org/packages/Syrx.Commanders.Databases/)**: Database command abstractions

## License

This project is licensed under the [MIT License](https://github.com/Syrx/Syrx/blob/main/LICENSE).

## Credits

Built on top of [System.Xml](https://docs.microsoft.com/en-us/dotnet/api/system.xml) and [Dapper](https://github.com/DapperLib/Dapper).



