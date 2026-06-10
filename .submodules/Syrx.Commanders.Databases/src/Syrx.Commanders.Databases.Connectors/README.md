````markdown
# Syrx.Commanders.Databases.Connectors

Database connector abstractions and base implementations for the Syrx data access framework.

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Installation](#installation)
- [Core Components](#core-components)
  - [IDatabaseConnector](#idatabaseconnector)
  - [DatabaseConnector](#databaseconnector)
- [Usage](#usage)
  - [Implementing a Custom Connector](#implementing-a-custom-connector)
  - [Using Existing Connectors](#using-existing-connectors)
- [Connection Management](#connection-management)
- [Database Provider Support](#database-provider-support)
- [Related Packages](#related-packages)
- [License](#license)
- [Credits](#credits)

## Overview

`Syrx.Commanders.Databases.Connectors` provides the foundational database connectivity abstractions for the Syrx framework. This package defines the interfaces and base classes that enable database-agnostic connection management across different database providers.

## Key Features

- **Database Abstraction**: Common interface for all database connections
- **Provider Pattern**: Pluggable database provider architecture
- **Connection Factory**: Standardized connection creation patterns
- **Type Safety**: Strongly-typed database provider definitions
- **Extensibility**: Easy to implement custom database connectors

## Installation

```bash
dotnet add package Syrx.Commanders.Databases.Connectors
```

**Package Manager**
```bash
Install-Package Syrx.Commanders.Databases.Connectors
```

**PackageReference**
```xml
<PackageReference Include="Syrx.Commanders.Databases.Connectors" Version="3.0.0" />
```

> **Note**: This package provides the abstractions only. You'll need database-specific connector packages for actual implementations.

## Core Components

### IDatabaseConnector

The main interface defining database connectivity:

```csharp
public interface IDatabaseConnector
{
    // Database connector abstraction
    // Implemented by specific database providers
}
```

### DatabaseConnector

Base implementation providing common functionality:

```csharp
public abstract class DatabaseConnector : IDatabaseConnector
{
    // Base connector implementation
    // Extended by database-specific connectors
    protected Func<DbProviderFactory> ProviderFactory { get; }
    
    protected DatabaseConnector(Func<DbProviderFactory> providerFactory)
    {
        ProviderFactory = providerFactory;
    }
}
```

## Usage

### Implementing a Custom Connector

```csharp
public class CustomDatabaseConnector : DatabaseConnector
{
    public CustomDatabaseConnector() 
        : base(() => CustomDbProviderFactory.Instance)
    {
    }
    
    // Implement custom database-specific logic
}
```

### Using Existing Connectors

```csharp
// SQL Server
services.AddScoped<IDatabaseConnector, SqlServerDatabaseConnector>();

// MySQL
services.AddScoped<IDatabaseConnector, MySqlDatabaseConnector>();

// PostgreSQL
services.AddScoped<IDatabaseConnector, NpgsqlDatabaseConnector>();

// Oracle
services.AddScoped<IDatabaseConnector, OracleDatabaseConnector>();
```

## Connection Management

Database connectors handle:

- **Provider Factory Management**: Manage `DbProviderFactory` instances
- **Connection String Resolution**: Resolve connection strings by alias
- **Connection Lifecycle**: Handle connection creation and disposal
- **Database-Specific Features**: Support provider-specific functionality

## Database Provider Support

The connector pattern supports any ADO.NET provider through the factory pattern:

```csharp
// SQL Server
() => SqlClientFactory.Instance

// MySQL (MySqlConnector)
() => MySqlConnectorFactory.Instance

// PostgreSQL
() => NpgsqlFactory.Instance

// Oracle
() => OracleClientFactory.Instance
```

## Related Packages

### Database-Specific Connectors
- **[Syrx.Commanders.Databases.Connectors.SqlServer](https://www.nuget.org/packages/Syrx.Commanders.Databases.Connectors.SqlServer/)**: SQL Server connector implementation
- **[Syrx.Commanders.Databases.Connectors.MySql](https://www.nuget.org/packages/Syrx.Commanders.Databases.Connectors.MySql/)**: MySQL connector implementation
- **[Syrx.Commanders.Databases.Connectors.Npgsql](https://www.nuget.org/packages/Syrx.Commanders.Databases.Connectors.Npgsql/)**: PostgreSQL connector implementation
- **[Syrx.Commanders.Databases.Connectors.Oracle](https://www.nuget.org/packages/Syrx.Commanders.Databases.Connectors.Oracle/)**: Oracle connector implementation

### Extension Packages
- **[Syrx.Commanders.Databases.Connectors.Extensions](https://www.nuget.org/packages/Syrx.Commanders.Databases.Connectors.Extensions/)**: Dependency injection extensions for connectors

### Core Framework
- **[Syrx.Commanders.Databases](https://www.nuget.org/packages/Syrx.Commanders.Databases/)**: Database command abstractions
- **[Syrx](https://www.nuget.org/packages/Syrx/)**: Core Syrx interfaces

## License

This project is licensed under the [MIT License](https://github.com/Syrx/Syrx/blob/main/LICENSE).

## Credits

Built on top of [Dapper](https://github.com/DapperLib/Dapper) and the ADO.NET provider model.
````



