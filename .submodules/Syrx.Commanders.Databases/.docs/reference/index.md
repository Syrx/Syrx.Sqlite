# Syrx.Commanders.Databases Reference Documentation

Comprehensive reference documentation for **Syrx.Commanders.Databases** — a high-performance, configuration-driven database access framework built on top of Dapper for .NET.

This reference covers all 11 projects, their public APIs, architecture patterns, configuration systems, and integration patterns.

---

## Quick Navigation

### 🚀 Getting Started
- [Getting Started Guide](getting-started.md) — New developer on-ramp, minimal setup example
- [Code Structure](code-structure.md) — Visual project architecture and dependency graph

### 🏗️ Architecture & Design
- [Architecture Overview](architecture/index.md) — Core design principles and patterns
- [Command Resolution](architecture/command-resolution.md) — Namespace.Type.Method → SQL mapping
- [Connection Management](architecture/connection-management.md) — Pooling, lifetimes, thread-safety
- [Transaction Handling](architecture/transaction-handling.md) — Semantics for Execute vs. Query operations
- [Thread Safety](architecture/thread-safety.md) — Concurrency guarantees across all components

### ⚙️ Configuration
- [Configuration Overview](configuration/index.md) — Configuration structure and fundamentals
- [Builder API Reference](configuration/builder-api.md) — Recommended fluent programmatic configuration
- [Configuration Examples](configuration/examples.md) — Complete working configuration samples
- [JSON Schema Reference](configuration/json-schema.md) — JSON configuration format with examples
- [XML Schema Reference](configuration/xml-schema.md) — XML configuration format with examples

### 📦 Project Reference (11 Projects)

#### Core & Execution
- [Syrx.Commanders.Databases](projects/commanders-databases.md) — Primary command execution engine
- [Syrx.Commanders.Databases.Builders](projects/builders.md) — Database schema modeling

#### Data Access Abstraction
- [Syrx.Commanders.Databases.Connectors](projects/connectors.md) — Connection abstraction layer
- [Syrx.Commanders.Databases.Connectors.Extensions](projects/connectors-extensions.md) — Connector DI setup

#### Configuration Management
- [Syrx.Commanders.Databases.Settings](projects/settings.md) — Configuration domain models
- [Syrx.Commanders.Databases.Settings.Extensions](projects/settings-extensions.md) — Fluent builder API
- [Syrx.Commanders.Databases.Settings.Extensions.Json](projects/settings-extensions-json.md) — JSON configuration loader
- [Syrx.Commanders.Databases.Settings.Extensions.Xml](projects/settings-extensions-xml.md) — XML configuration loader
- [Syrx.Commanders.Databases.Settings.Readers](projects/settings-readers.md) — Command resolution engine
- [Syrx.Commanders.Databases.Settings.Readers.Extensions](projects/settings-readers-extensions.md) — Reader DI setup

#### Dependency Injection
- [Syrx.Commanders.Databases.Extensions](projects/extensions.md) — Core DI extensions

### 📊 Quality & Coverage
- [Coverage Report](coverage-report.md) — Documentation completeness audit, gap analysis, and metrics

---

## Core Concepts at a Glance

### The Repository Pattern
```csharp
// Your repository receives ICommander<TYourRepository>
public class UserRepository
{
    private readonly ICommander<UserRepository> _commander;
    
    public UserRepository(ICommander<UserRepository> commander)
    {
        _commander = commander;
    }
    
    public async Task<User?> RetrieveAsync(int id, CancellationToken cancellationToken = default)
    {
      var result = await _commander.QueryAsync<User>(new { id }, cancellationToken);
        return result.FirstOrDefault();
    }
}
```

### Configuration-Driven SQL
Database commands are externalized from code using **namespace.type.method** resolution:

```json
{
  "namespaces": [
    {
      "namespace": "MyApp.Data.Repositories",
      "types": [
        {
          "name": "UserRepository",
          "commands": {
            "RetrieveAsync": {
              "commandText": "SELECT * FROM Users WHERE Id = @id",
              "connectionAlias": "Default"
            }
          }
        }
      ]
    }
  ]
}
```

### Multi-Mapping & Complex Queries
```csharp
// Query with multi-mapping (e.g., Users joined with Profiles)
var result = await _commander.QueryAsync<User, Profile, UserWithProfile>(
    (user, profile) => new UserWithProfile { User = user, Profile = profile },
    parameters: new { isActive = true }
);
```

---

## Architecture at a Glance

### Layered Design
```
┌─────────────────────────────────────────┐
│  Your Repository Code                   │
│  (ICommand<TRepository> injection)      │
└──────────────┬──────────────────────────┘
               │
┌──────────────▼──────────────────────────┐
│  DatabaseCommander<TRepository>         │
│  (Command execution engine)             │
├──────────────────────────────────────── │
│ - Resolves repository method → SQL cmd  │
│ - Handles connection & transaction mgmt │
│ - Supports all mapping variants         │
└──────────────┬──────────────────────────┘
               │
┌──────────────┼──────────────────────────┐
│  Configuration Layer                    │
├──────────────┼──────────────────────────┤
│ IDatabaseCommandReader                  │
│ ICommanderSettings (JSON/XML/Builder)   │
└──────────────┼──────────────────────────┘
               │
┌──────────────┼──────────────────────────┐
│  Data Access Layer                      │
├──────────────┼──────────────────────────┤
│ IDatabaseConnector                      │
│ Database Connections (via DbFactory)    │
│ Dapper ORM Integration                  │
└──────────────▼──────────────────────────┘
               │
          SQL Database
```

---

## Key Principles

### Design Philosophy
- **Configuration-Driven**: SQL lives in configuration files, not code
- **Least Privilege**: Only expose what repositories need (ICommander<T>)
- **Type-Safe**: Compile-time validation of method names via [CallerMemberName]
- **Async-First**: Async operations preferred; sync supported
- **Thread-Safe**: All caching and concurrent operations are thread-safe  
- **Performance**: Caching at three levels: commands, connections, compiled queries

### Multi-Provider Support
Supports any ADO.NET provider via DbProviderFactory:
- SQL Server (SqlClient)
- PostgreSQL (Npgsql)
- MySQL (MySql.Data)
- SQLite
- Oracle
- Others via custom provider factories

---

## Documentation Structure

| Section | Purpose | For Whom |
|---------|---------|----------|
| [Getting Started](getting-started.md) | First-time setup and minimal example | New developers, evaluators |
| [Architecture](architecture/index.md) | Design principles, patterns, constraints | Architects, lead developers |
| [Configuration](configuration/index.md) | Complete reference for all config formats | Integrators, configuration owners |
| [Projects](projects/index.md) | API reference for each of the 11 projects | API consumers, maintainers |
| [Coverage](coverage-report.md) | Documentation audit and gap analysis | Documentation owners, QA |

---

## Common Tasks

### I want to...

- **Set up Syrx in a new application** → [Getting Started](getting-started.md)
- **Understand how commands are resolved** → [Command Resolution](architecture/command-resolution.md)
- **Configure JSON or XML settings** → [Configuration Overview](configuration/index.md)
- **Learn the builder API** → [Builder API Reference](configuration/builder-api.md)
- **Understand connection management** → [Connection Management](architecture/connection-management.md)
- **Use multi-mapping queries** → [Syrx.Commanders.Databases Reference](projects/commanders-databases.md)
- **Check documentation completeness** → [Coverage Report](coverage-report.md)
- **Deep dive into a specific project** → [Project Reference](projects/index.md)

---

## API Quick Reference

### Core Interfaces
- **`ICommander<TRepository>`** — Primary interface for executing commands (implemented by `DatabaseCommander<TRepository>`)
- **`IDatabaseConnector`** — Creates database connections from command settings
- **`ICommanderSettings`** — Configuration container (typically loaded from JSON/XML)
- **`IDatabaseCommandReader`** — Internal service for command resolution (framework use)

### Configuration Types
- **`CommanderSettings`** — Root configuration record
- **`NamespaceSetting`** — Namespace-level command grouping
- **`TypeSetting`** — Type-level command grouping (repository type)
- **`CommandSetting`** — Individual command configuration with SQL text, timeout, flags, etc.
- **`ConnectionStringSetting`** — Named connection string configuration

### Builder Classes (Fluent API)
- **`CommanderSettingsBuilder`** — Assembles configuration programmatically
- **`NamespaceSettingBuilder`** — Builds namespace-level settings
- **`TypeSettingBuilder`** — Builds type-level settings
- **`CommandSettingBuilder`** — Builds command settings

---

## Dependency Injection Setup

### Minimal Setup (ASP.NET Core)

```csharp
// Program.cs
builder.Services.UseSyrx(config => 
    config.UseFile("syrx.json", settings => { /* optional additional config */ })
);

// Register your repository
builder.Services.AddScoped<UserRepository>();
```

### Multiple Databases

```csharp
builder.Services.UseSyrx(config =>
{
    config
        .AddConnectionString("Main", "Server=primary;...")
        .AddConnectionString("Reporting", "Server=reporting;...")
        .UseFile("syrx.json", additionalSetup => { });
});
```

---

## Support & Resources

- **Source:** [GitHub Repository](https://github.com/Syrx/Syrx.Commanders.Databases)
- **Issues:** [GitHub Issues](https://github.com/Syrx/Syrx.Commanders.Databases/issues)
- **License:** [LICENSE.txt](../../LICENSE)

---

**Documentation Generated**: March 28, 2026  
**Coverage**: 11 projects, 45+ public types, comprehensive API reference  
**Status**: ✅ Complete — see [Coverage Report](coverage-report.md) for details



