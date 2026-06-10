# Syrx.Commanders.Databases.Builders

Database schema modeling and builder utilities for the Syrx framework.

## Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Installation](#installation)
- [Core Components](#core-components)
  - [Database](#database)
  - [Table](#table)
  - [Field](#field)
- [Usage](#usage)
  - [Basic Schema Modeling](#basic-schema-modeling)
  - [Builder Pattern Usage](#builder-pattern-usage)
- [Integration](#integration)
- [Related Packages](#related-packages)
- [License](#license)
- [Credits](#credits)

## Overview

`Syrx.Commanders.Databases.Builders` provides schema modeling and builder pattern utilities for working with database structures in the Syrx framework. This package enables programmatic definition of database schemas using strongly-typed builders and is particularly useful for code generation, schema analysis, and database tooling scenarios.

## Key Features

- **Schema Modeling**: Programmatic definition of database, table, and field structures
- **Builder Pattern**: Fluent API for constructing database schemas
- **Type Safety**: Strongly-typed schema definitions with validation
- **Extensible**: Easy to extend for custom schema modeling needs
- **Integration Ready**: Works seamlessly with other Syrx components

## Installation

```bash
dotnet add package Syrx.Commanders.Databases.Builders
```

**Package Manager**
```bash
Install-Package Syrx.Commanders.Databases.Builders
```

**PackageReference**
```xml
<PackageReference Include="Syrx.Commanders.Databases.Builders" Version="3.0.0" />
```

## Core Components

### Database

Represents a complete database schema with tables:

```csharp
public class Database
{
    public string Name { get; }
    public IEnumerable<Table> Tables { get; }
}
```

### Table

Represents a database table with fields and metadata:

```csharp
public class Table
{
    public string Name { get; }
    public IEnumerable<Field> Fields { get; }
    // Additional table properties
}
```

### Field

Represents a database field/column with type information:

```csharp
public class Field
{
    public string Name { get; }
    public Type DataType { get; }
    // Additional field properties
}
```

## Usage

### Basic Schema Modeling

```csharp
// Create field definitions
var idField = new Field("Id", typeof(int), /* additional options */);
var nameField = new Field("Name", typeof(string), /* additional options */);
var emailField = new Field("Email", typeof(string), /* additional options */);

// Create table with fields
var userTable = new Table("Users", new[] { idField, nameField, emailField });

// Create database with tables
var database = new Database("MyApplication", new[] { userTable });
```

### Builder Pattern Usage

The builders provide fluent APIs for constructing schemas:

```csharp
// Using builder extensions (when available)
var database = DatabaseBuilder.Create("MyApplication")
    .AddTable("Users", table => table
        .AddField("Id", typeof(int), field => field.SetPrimaryKey())
        .AddField("Name", typeof(string), field => field.SetMaxLength(100))
        .AddField("Email", typeof(string), field => field.SetMaxLength(255)))
    .AddTable("Products", table => table
        .AddField("Id", typeof(int), field => field.SetPrimaryKey())
        .AddField("Name", typeof(string))
        .AddField("Price", typeof(decimal)))
    .Build();
```

### Options Configuration

Configure field and table options:

```csharp
// Field options
var fieldOptions = new FieldOptions
{
    IsNullable = false,
    MaxLength = 100,
    IsPrimaryKey = true
};

// Table options  
var tableOptions = new TableOptions
{
    Schema = "dbo",
    HasPrimaryKey = true
};

// Database options
var databaseOptions = new DatabaseOptions
{
    DefaultSchema = "dbo",
    ConnectionString = "Server=localhost;Database=MyApp;..."
};
```

## Integration

### Code Generation

Use the schema models for code generation scenarios:

```csharp
public class EntityGenerator
{
    public string GenerateEntity(Table table)
    {
        var className = table.Name.Singularize();
        var properties = table.Fields.Select(GenerateProperty);
        
        return $@"
public class {className}
{{
{string.Join("\n", properties)}
}}";
    }
    
    private string GenerateProperty(Field field)
    {
        var typeName = RetrieveCSharpTypeName(field.DataType);
        return $"    public {typeName} {field.Name} {{ get; set; }}";
    }
}
```

### Schema Analysis

Analyze database structures:

```csharp
public class SchemaAnalyzer
{
    public void AnalyzeDatabase(Database database)
    {
        Console.WriteLine($"Database: {database.Name}");
        Console.WriteLine($"Tables: {database.Tables.Count()}");
        
        foreach (var table in database.Tables)
        {
            Console.WriteLine($"  Table: {table.Name}");
            Console.WriteLine($"    Fields: {table.Fields.Count()}");
            
            foreach (var field in table.Fields)
            {
                Console.WriteLine($"      {field.Name}: {field.DataType.Name}");
            }
        }
    }
}
```

### Integration with Other Syrx Components

```csharp
// Use with command generation
public class CommandGenerator
{
    public CommandSetting GenerateSelectCommand(Table table)
    {
        var sql = $"SELECT {string.Join(", ", table.Fields.Select(f => f.Name))} FROM {table.Name}";
        
        return new CommandSetting
        {
            CommandText = sql,
            CommandType = CommandType.Text,
            ConnectionAlias = "Default"
        };
    }
}
```

## Related Packages

### Core Framework
- **[Syrx.Commanders.Databases](https://www.nuget.org/packages/Syrx.Commanders.Databases/)**: Core database command abstractions
- **[Syrx.Commanders.Databases.Settings](https://www.nuget.org/packages/Syrx.Commanders.Databases.Settings/)**: Configuration settings
- **[Syrx](https://www.nuget.org/packages/Syrx/)**: Core interfaces and abstractions

### Extensions
- **[Syrx.Commanders.Databases.Extensions](https://www.nuget.org/packages/Syrx.Commanders.Databases.Extensions/)**: Service registration extensions
- **[Syrx.Commanders.Databases.Settings.Extensions](https://www.nuget.org/packages/Syrx.Commanders.Databases.Settings.Extensions/)**: Settings builder extensions

## Use Cases

This package is particularly useful for:

- **Code Generation Tools**: Generate entities, repositories, or commands from schema definitions
- **Database Tooling**: Build database management and migration tools
- **Schema Documentation**: Document database structures programmatically
- **Testing Utilities**: Create test database schemas for integration tests
- **ORM Mapping**: Generate object-relational mappings from schema models

## License

This project is licensed under the [MIT License](https://github.com/Syrx/Syrx/blob/main/LICENSE).

## Credits

Built as part of the Syrx data access framework to provide schema modeling capabilities for database-centric applications.



