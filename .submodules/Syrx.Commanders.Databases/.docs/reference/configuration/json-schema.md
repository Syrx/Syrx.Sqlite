# JSON Schema Reference

Complete JSON file-based configuration schema for Syrx.Commanders.Databases with examples. For recommended builder-first guidance, see the Builder API reference.

---

## Schema Structure

```json
{
  "connections": [
    {
      "alias": "string",
      "connectionString": "string"
    }
  ],
  "namespaces": [
    {
      "namespace": "string",
      "types": [
        {
          "name": "string",
          "commands": {
            "methodName": {
              "commandText": "string",
              "connectionAlias": "string",
              "commandTimeout": "number (optional)",
              "commandType": "string (optional)",
              "flags": "array (optional)",
              "isolationLevel": "string (optional)",
              "split": "string (optional)"
            }
          }
        }
      ]
    }
  ]
}
```

---

## Minimal Example

```json
{
  "connections": [
    {
      "alias": "Default",
      "connectionString": "Server=localhost;Database=MyDb;Integrated Security=true;Encrypt=true;TrustServerCertificate=false;"
    }
  ],
  "namespaces": [
    {
      "namespace": "MyApp.Data",
      "types": [
        {
          "name": "UserRepository",
          "commands": {
            "RetrieveAsync": {
              "commandText": "SELECT Id, Name, Email FROM Users WHERE Id = @id",
              "connectionAlias": "Default"
            }
          }
        }
      ]
    }
  ]
}
```

---

## Complete Example (All Options)

```json
{
  "connections": [
    {
      "alias": "Primary",
      "connectionString": "Server=primary-db.example.com;Database=MainDb;User Id=appuser;Password=${DB_PASSWORD};"
    },
    {
      "alias": "Reporting",
      "connectionString": "Server=reporting-db.example.com;Database=Reports;User Id=reportuser;Password=${DB_PASSWORD};"
    }
  ],
  "namespaces": [
    {
      "namespace": "MyApp.Core.Data",
      "types": [
        {
          "name": "UserRepository",
          "commands": {
            "RetrieveAsync": {
              "commandText": "SELECT Id, Name, Email, CreatedAt FROM Users WHERE Id = @id",
              "connectionAlias": "Primary",
              "commandTimeout": 30,
              "commandType": "Text",
              "flags": ["Buffered"],
              "isolationLevel": "Serializable",
              "split": null
            },
            "CreateAsync": {
              "commandText": "INSERT INTO Users (Name, Email, CreatedAt) VALUES (@name, @email, @createdAt)",
              "connectionAlias": "Primary",
              "commandTimeout": 30,
              "commandType": "Text",
              "isolationLevel": "Serializable"
            },
            "RetrieveWithProfiles": {
              "commandText": "SELECT u.*, p.* FROM Users u JOIN Profiles p ON u.Id = p.UserId",
              "connectionAlias": "Primary",
              "split": "Id"
            }
          }
        },
        {
          "name": "ReportRepository",
          "commands": {
            "RetrieveMonthlyReportAsync": {
              "commandText": "sp_GenerateMonthlyReport",
              "connectionAlias": "Reporting",
              "commandType": "StoredProcedure",
              "commandTimeout": 300
            }
          }
        }
      ]
    }
  ]
}
```

---

## Property Reference

### connections

Array of connection configurations.

```json
{
  "connections": [
    {
      "alias": "Default",
      "connectionString": "Server=localhost;Database=MyDb;Integrated Security=true;"
    }
  ]
}
```

- **alias** (string, required): Unique identifier for this connection
- **connectionString** (string, required): ADO.NET connection string

### namespaces

Array of namespace configurations (mirrors your C# namespace structure).

```json
{
  "namespaces": [
    {
      "namespace": "MyApp.Data",
      "types": [...]
    }
  ]
}
```

- **namespace** (string, required): C# namespace name (case-sensitive)
- **types** (array, required): Array of type configurations

### types

Array of type configurations (mirrors your repository classes).

```json
{
  "name": "UserRepository",
  "commands": {...}
}
```

- **name** (string, required): C# class name (case-sensitive)
- **commands** (object, required): Object with method names as keys

### commands

Object with method names as keys; command configurations as values.

```json
{
  "RetrieveAsync": {...},
  "CreateAsync": {...}
}
```

Each key must match a public method name in your repository class (case-sensitive).

---

## Command Properties

### commandText

The SQL text or stored procedure name to execute.

```json
{
  "commandText": "SELECT * FROM Users WHERE Id = @id"
}
```

**Supports**:
- Multi-line SQL strings
- Parameter references (@paramName)
- Stored procedure names
- Table-valued functions

### connectionAlias

Required. Must reference a defined connection alias.

```json
{
  "connectionAlias": "Default"
}
```

### commandTimeout (optional)

Timeout in seconds. Default: 30.

```json
{
  "commandTimeout": 60
}
```

### commandType (optional)

Type of command. Default: "Text".

```json
{
  "commandType": "Text"                // ← Default
  // OR
  "commandType": "StoredProcedure"
  // OR
  "commandType": "TableDirect"        // Rarely used
}
```

### flags (optional)

Array of Dapper command flags. Default: ["Buffered"].

```json
{
  "flags": ["Buffered", "NoCache"]     // ← Default
  // OR
  "flags": ["Buffered"]
  // OR
 "flags": ["Pipelined"]
  // OR
  "flags": []                          // No flags
}
```

**Options**:
- **Buffered**: Cache results in memory (recommended for most queries)
- **Pipelined**: Execute multiple queries without waiting (advanced)
- **NoCache**: Don't cache query plan (usually enabled by default)

### isolationLevel (optional)

Transaction isolation level for Execute operations. Default: "Serializable".

```json
{
  "isolationLevel": "Serializable"     // ← Strictest, default
  // OR
  "isolationLevel": "ReadCommitted"
  // OR
  "isolationLevel": "RepeatableRead"
  // OR
  "isolationLevel": "ReadUncommitted"  // Least strict
  // OR
  "isolationLevel": "Snapshot"
  // OR
  "isolationLevel": "Chaos"
}
```

### split (optional)

Column name used by Dapper to split multi-mapped result sets. Default: "Id".

```json
{
  "split": "Id"
}
```

Only needed for multi-mapping queries (2+ types per row).

---

## Real-World Configuration Files

### Single Database

```json
{
  "connections": [{
    "alias": "Default",
    "connectionString": "Server=localhost;Database=AppDb;Integrated Security=true;TrustServerCertificate=false;"
  }],
  "namespaces": [{
    "namespace": "MyApp.Data",
    "types": [{
      "name": "UserRepository",
      "commands": {
        "RetrieveAsync": {
          "commandText": "SELECT * FROM Users WHERE Id = @id",
          "connectionAlias": "Default"
        }
      }
    }]
  }]
}

### Multi-Database Setup

```json
{
  "connections": [
    {
      "alias": "Main",
      "connectionString": "Server=primary.example.com;Database=Main;User Id=app;Password=${MAIN_DB_PASSWORD};"
    },
    {
      "alias": "Analytics",
      "connectionString": "Server=analytics.example.com;Database=Analytics;User Id=app;Password=${ANALYTICS_DB_PASSWORD};"
    }
  ],
  "namespaces": [
    {
      "namespace": "MyApp.Data",
      "types": [
        {
          "name": "UserRepository",
          "commands": {
            "RetrieveUser": { "commandText": "SELECT * FROM Users WHERE Id = @id", "connectionAlias": "Main" }
          }
        },
        {
          "name": "AnalyticsRepository",
          "commands": {
            "RetrieveMetrics": { "commandText": "SELECT * FROM Metrics", "connectionAlias": "Analytics" }
          }
        }
      ]
    }
  ]
}
```

---

## Validation Rules

1. All aliases in commands must be defined in connections
2. All namespaces must match C# namespace names exactly
3. All type names must match repository class names exactly
4. All command keys must match repository method names exactly
5. connectionText must not be empty or null
6. connectionAlias must not be empty or null

---

## Next Steps

- [Builder API Reference](builder-api.md) — Recommended fluent programmatic configuration
- [XML Schema Reference](xml-schema.md) — XML format alternative
- [Configuration Examples](examples.md) — More complete samples



