# Syrx.Sqlite

Syrx.Sqlite adds SQLite support to the Syrx data-access stack while preserving the explicit command model used by `ICommander<TRepository>`.

## Overview

- Target framework: .NET 10
- Current package version: 3.0.0
- Primary dependency: Microsoft.Data.Sqlite
- Recommended entry point: Syrx.Sqlite.Extensions

## Packages

| Package | Purpose |
|--|--|
| Syrx.Commanders.Databases.Connectors.Sqlite | Low-level SQLite database connector implementation. |
| Syrx.Commanders.Databases.Connectors.Sqlite.Extensions | Dependency injection extensions for registering the connector. |
| Syrx.Sqlite | Aggregates the core SQLite support packages. |
| Syrx.Sqlite.Extensions | Recommended package for most consumers; combines Syrx and SQLite registration helpers. |

## Installation

### Recommended package

| Source | Command |
|--|--|
| .NET CLI | `dotnet add package Syrx.Sqlite.Extensions --version 3.0.0` |
| Package Manager | `Install-Package Syrx.Sqlite.Extensions -Version 3.0.0` |
| Package Reference | `<PackageReference Include="Syrx.Sqlite.Extensions" Version="3.0.0" />` |
| Paket CLI | `paket add Syrx.Sqlite.Extensions --version 3.0.0` |

## Usage

```csharp
using Syrx.Commanders.Databases.Connectors.Sqlite.Extensions;

public static IServiceCollection Install(this IServiceCollection services)
{
    return services.UseSyrx(builder =>
        builder.UseSqlite(settings => settings
            .AddConnectionString("Default", "Data Source=app.db;")
            .AddCommand(commandNamespace =>
            {
                // Register explicit Syrx command definitions for each repository method.
            })));
}
```

## Build and test

```powershell
dotnet build Syrx.Sqlite.sln --configuration Release
dotnet test Syrx.Sqlite.sln --configuration Release
```

SQLite integration tests use a local temporary file database instead of containers. SQLite is embedded and does not require a server process, so Testcontainers is unnecessary for reliable integration coverage.
