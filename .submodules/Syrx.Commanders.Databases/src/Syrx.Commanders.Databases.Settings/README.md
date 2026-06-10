# Syrx.Commanders.Databases.Settings

Configuration settings and models for the Syrx database framework.

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Installation](#installation)
- [Core Settings Classes](#core-settings-classes)
  - [CommanderSettings](#commandersettings)
  - [CommandSetting](#commandsetting)
  - [ConnectionStringSetting](#connectionstringsetting)
  - [NamespaceSetting](#namespacesetting)
  - [TypeSetting](#typesetting)
- [Configuration Hierarchy](#configuration-hierarchy)
- [Usage](#usage)
  - [Basic Configuration](#basic-configuration)
  - [Programmatic Configuration](#programmatic-configuration)
  - [JSON Configuration](#json-configuration)
  - [XML Configuration](#xml-configuration)
- [Command Configuration](#command-configuration)
  - [Command Types](#command-types)
  - [Connection Aliases](#connection-aliases)
  - [Timeout Settings](#timeout-settings)
  - [Transaction Isolation](#transaction-isolation)
- [Connection String Management](#connection-string-management)
- [Settings Validation](#settings-validation)
- [Related Packages](#related-packages)
- [License](#license)
- [Credits](#credits)

## Overview

`Syrx.Commanders.Databases.Settings` provides comprehensive configuration settings and models for the Syrx database framework. This package defines the structure and types used to configure database connections, commands, and execution parameters.

## Key Features

- **Hierarchical Configuration**: Organized by namespace, type, and method
- **Type Safety**: Strongly-typed configuration classes
- **Flexible Command Settings**: Support for various SQL command types and parameters
- **Connection Management**: Multiple connection string aliases
- **Validation Support**: Built-in configuration validation
- **Extensible**: Support for custom configuration extensions

## Installation

```bash
dotnet add package Syrx.Commanders.Databases.Settings
```

**Package Manager**
```bash
Install-Package Syrx.Commanders.Databases.Settings
```

**PackageReference**
```xml
<PackageReference Include="Syrx.Commanders.Databases.Settings" Version="3.0.0" />
```

## Core Settings Classes

### CommanderSettings

The root settings container implementing `ICommanderSettings`:

```csharp
public sealed record CommanderSettings : ICommanderSettings
{
    public required List<NamespaceSetting> Namespaces { get; init; }
    public List<ConnectionStringSetting>? Connections { get; init; }
}
```

### CommandSetting

Individual command configuration:

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

### ConnectionStringSetting

Connection string configuration:

```csharp
public sealed record ConnectionStringSetting
{
    public required string ConnectionString { get; init; }
    public required string Alias { get; init; }
}
```

### NamespaceSetting

Namespace-level configuration container:

```csharp
public sealed record NamespaceSetting
{
    public required string Namespace { get; init; }
    public required List<TypeSetting> Types { get; init; }
}
```

### TypeSetting

Type-level configuration container:

```csharp
public sealed record TypeSetting
{
    public required string Name { get; init; }
    public required Dictionary<string, CommandSetting> Commands { get; init; }
}
```

## Configuration Hierarchy

The settings follow a hierarchical structure that mirrors your code organization:

```
CommanderSettings
├── Connections[]
│   ├── Alias
│   └── ConnectionString
└── Namespaces[]
    ├── Name (Namespace)
    └── Types[]
        ├── Name (Class)
        └── Commands{}
            └── Key: Method Name
                └── Value: CommandSetting
```

## Usage

`Syrx.Commanders.Databases.Settings.Extensions` is the recommended way to author settings for application startup. The examples below include raw model construction and file formats to illustrate shape equivalence across sources.

### Basic Configuration

```csharp
var settings = new CommanderSettings
{
    Connections = new[]
    {
        new ConnectionStringSetting
        {
            Alias = "DefaultConnection",
            ConnectionString = "Server=localhost;Database=MyApp;Trusted_Connection=true;"
        }
    },
    Namespaces = new[]
    {
        new NamespaceSetting
        {
            Name = "MyApp.Repositories",
            Types = new[]
            {
                new TypeSetting
                {
                    Name = "UserRepository",
                    Commands = new Dictionary<string, CommandSetting>
                    {
                        ["RetrieveAsync"] = new CommandSetting
                        {
                            CommandText = "SELECT * FROM Users WHERE Id = @id",
                            ConnectionAlias = "DefaultConnection",
                            CommandTimeout = 30
                        }
                    }
                }
            }
        }
    }
};
```

### Programmatic Configuration

```csharp
public class ConfigurationService
{
    public CommanderSettings BuildSettings()
    {
        var settings = new CommanderSettings();
        
        // Add connection strings
        settings.Connections = new[]
        {
            new ConnectionStringSetting 
            { 
                Alias = "Primary", 
                ConnectionString = RetrievePrimaryConnectionString() 
            },
            new ConnectionStringSetting 
            { 
                Alias = "ReadOnly", 
                ConnectionString = RetrieveReadOnlyConnectionString() 
            }
        };
        
        // Configure namespaces and commands
        settings.Namespaces = BuildNamespaceSettings();
        
        return settings;
    }
}
```

### JSON Configuration

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
              "CommandTimeout": 30,
              "CommandType": "Text"
            }
          }
        }
      ]
    }
  ]
}
```

### XML Configuration

```xml
<CommanderSettings>
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
          </Commands>
        </TypeSetting>
      </Types>
    </NamespaceSetting>
  </Namespaces>
</CommanderSettings>
```

## Command Configuration

### Command Types

```csharp
new CommandSetting
{
    CommandType = CommandType.Text,        // SQL query (default)
    CommandType = CommandType.StoredProcedure,  // Stored procedure
    CommandType = CommandType.TableDirect       // Direct table access
}
```

### Connection Aliases

```csharp
new CommandSetting
{
    ConnectionAlias = "Primary",    // Use primary database
    ConnectionAlias = "ReadOnly",   // Use read-only replica
    ConnectionAlias = "Analytics"   // Use analytics database
}
```

### Timeout Settings

```csharp
new CommandSetting
{
    CommandTimeout = 30,    // 30 seconds (default)
    CommandTimeout = 60,    // 1 minute for complex queries
    CommandTimeout = 300    // 5 minutes for long operations
}
```

### Transaction Isolation

```csharp
new CommandSetting
{
    IsolationLevel = IsolationLevel.ReadCommitted,  // Default
    IsolationLevel = IsolationLevel.ReadUncommitted,
    IsolationLevel = IsolationLevel.RepeatableRead,
    IsolationLevel = IsolationLevel.Serializable
}
```

## Connection String Management

Multiple connection strings for different purposes:

```csharp
var settings = new CommanderSettings
{
    Connections = new[]
    {
        // Primary read-write connection
        new ConnectionStringSetting
        {
            Alias = "Primary",
            ConnectionString = "Server=primary;Database=MyApp;..."
        },
        
        // Read-only replica
        new ConnectionStringSetting
        {
            Alias = "ReadOnly", 
            ConnectionString = "Server=readonly;Database=MyApp;..."
        },
        
        // Analytics database
        new ConnectionStringSetting
        {
            Alias = "Analytics",
            ConnectionString = "Server=analytics;Database=Reporting;..."
        }
    }
};
```

## Configuration Notes

The settings classes use `required` properties and immutable records to ensure configuration integrity at compile time. All required fields must be provided when creating settings instances.

## Related Packages

### Configuration Extensions
- **[Syrx.Commanders.Databases.Settings.Extensions](https://www.nuget.org/packages/Syrx.Commanders.Databases.Settings.Extensions/)**: Recommended builder pattern extensions
- **[Syrx.Commanders.Databases.Settings.Extensions.Json](https://www.nuget.org/packages/Syrx.Commanders.Databases.Settings.Extensions.Json/)**: Optional JSON configuration support
- **[Syrx.Commanders.Databases.Settings.Extensions.Xml](https://www.nuget.org/packages/Syrx.Commanders.Databases.Settings.Extensions.Xml/)**: Optional XML configuration support

### Configuration Readers
- **[Syrx.Commanders.Databases.Settings.Readers](https://www.nuget.org/packages/Syrx.Commanders.Databases.Settings.Readers/)**: Configuration file readers
- **[Syrx.Commanders.Databases.Settings.Readers.Extensions](https://www.nuget.org/packages/Syrx.Commanders.Databases.Settings.Readers.Extensions/)**: Reader extensions

### Core Framework
- **[Syrx.Settings](https://www.nuget.org/packages/Syrx.Settings/)**: Base configuration interfaces
- **[Syrx.Commanders.Databases](https://www.nuget.org/packages/Syrx.Commanders.Databases/)**: Database command abstractions

## License

This project is licensed under the [MIT License](https://github.com/Syrx/Syrx/blob/main/LICENSE).

## Credits

Built on top of [Dapper](https://github.com/DapperLib/Dapper) and standard .NET configuration patterns.



