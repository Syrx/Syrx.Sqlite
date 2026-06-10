# Getting Started with Syrx.Commanders.Databases

Get up and running with Syrx in under 10 minutes. This guide walks you through a minimal setup for a simple application.

---

## Prerequisites

- .NET 8.0 or later
- SQL Server or compatible ADO.NET provider
- Basic C# and ASP.NET Core knowledge

---

## Installation

### 1. Add NuGet Packages

```bash
# Core package
dotnet add package Syrx.Commanders.Databases

# Recommended configuration package (builder pattern)
dotnet add package Syrx.Commanders.Databases.Settings.Extensions

# Optional file-based configuration loaders
dotnet add package Syrx.Commanders.Databases.Settings.Extensions.Json
dotnet add package Syrx.Commanders.Databases.Settings.Extensions.Xml

# DI extensions (automatically required)
dotnet add package Syrx.Commanders.Databases.Extensions
dotnet add package Syrx.Commanders.Databases.Connectors.Extensions
dotnet add package Syrx.Commanders.Databases.Settings.Readers.Extensions

# Or install all at once
dotnet add package Syrx.Commanders.Databases
```

Builder-based configuration from `Syrx.Commanders.Databases.Settings.Extensions` is the recommended default. JSON and XML loaders are useful when your team prefers file-authored settings.

---

## Step 1: Create Your Repository

Define your repository interface and implementation:

```csharp
// Data/IUserRepository.cs
namespace MyApp.Data
{
    public interface IUserRepository
    {
        Task<User?> RetrieveAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> RetrieveAllAsync(int page = 1, int size = 100, CancellationToken cancellationToken = default);
      Task<bool> CreateAsync(User user, CancellationToken cancellationToken = default);
    }
}

// Data/UserRepository.cs
namespace MyApp.Data
{
    public class UserRepository : IUserRepository
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
        
        public async Task<IEnumerable<User>> RetrieveAllAsync(
            int page = 1,
            int size = 100,
            CancellationToken cancellationToken = default)
        {
            return await _commander.QueryAsync<User>(new { page, size }, cancellationToken);
        }
        
        public async Task<bool> CreateAsync(User user, CancellationToken cancellationToken = default)
        {
            // Execute command (INSERT, UPDATE, DELETE) returns success/failure
            return await _commander.ExecuteAsync(user, cancellationToken);
        }
    }
}

// Models/User.cs
public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

---

## Step 2: Build Configuration in Code (Recommended)

Create settings with the Extensions builder APIs:

```csharp
using Syrx.Commanders.Databases.Settings;
using Syrx.Commanders.Databases.Settings.Extensions;

var settings = CommanderSettingsBuilderExtensions.Build(builder => builder
  .AddConnectionString("Default", "Server=localhost;Database=MyDatabase;Integrated Security=true;Encrypt=true;TrustServerCertificate=false;")
  .AddCommand(ns => ns.ForType<UserRepository>(type => type
    .ForMethod(nameof(UserRepository.RetrieveAsync), cmd => cmd
      .UseCommandText("SELECT Id, Name, Email, CreatedAt FROM Users WHERE Id = @id")
      .UseConnectionAlias("Default"))
    .ForMethod(nameof(UserRepository.RetrieveAllAsync), cmd => cmd
      .UseCommandText("SELECT Id, Name, Email, CreatedAt FROM Users ORDER BY Name OFFSET ((@page - 1) * @size) ROWS FETCH NEXT @size ROWS ONLY")
      .UseConnectionAlias("Default"))
    .ForMethod(nameof(UserRepository.CreateAsync), cmd => cmd
      .UseCommandText("INSERT INTO Users (Name, Email, CreatedAt) VALUES (@name, @email, @createdAt)")
      .UseConnectionAlias("Default")
      .SetCommandTimeout(30)))));
```

**Key Points:**
- `ForType<T>()` and `ForMethod(...)` map directly to repository type and method names
- Builder guards fail fast for invalid configuration values
- `UseCommandText(...)` and `UseConnectionAlias(...)` map to `CommandSetting`
- Build once at startup and register the resulting `ICommanderSettings`

---

## Step 2B: File-Based Configuration (Optional)

If your team prefers file-authored settings, create a `syrx.json` file at your application root (or another convenient location):

```json
{
  "connections": [
    {
      "alias": "Default",
      "connectionString": "Server=localhost;Database=MyDatabase;Integrated Security=true;Encrypt=true;TrustServerCertificate=false;"
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
              "commandText": "SELECT Id, Name, Email, CreatedAt FROM Users WHERE Id = @id",
              "connectionAlias": "Default"
            },
            "RetrieveAllAsync": {
              "commandText": "SELECT Id, Name, Email, CreatedAt FROM Users ORDER BY Name OFFSET ((@page - 1) * @size) ROWS FETCH NEXT @size ROWS ONLY",
              "connectionAlias": "Default"
            },
            "CreateAsync": {
              "commandText": "INSERT INTO Users (Name, Email, CreatedAt) VALUES (@name, @email, @createdAt); SELECT CAST(SCOPE_IDENTITY() AS INT);",
              "connectionAlias": "Default",
              "commandTimeout": 30
            }
          }
        }
      ]
    }
  ]
}
```

**Key Points:**
- `namespace` matches your C# namespace exactly
- `name` matches your repository class name exactly  
- `commands` keys usually match your method names exactly (for overloads, pass an explicit `method:` value on the direct `_commander` call)
- Connection strings are identified by `alias` (e.g., "Default")
- `commandText` contains your actual SQL (text commands are the default)
- Collection queries should page results explicitly; SQL Server uses `OFFSET` and `FETCH NEXT`

---

## Step 3: Configure Dependency Injection

In your ASP.NET Core startup (Program.cs):

```csharp
using Microsoft.Extensions.DependencyInjection;
using Syrx.Commanders.Databases.Extensions;
using MyApp.Data;

var builder = WebApplicationBuilder.CreateBuilder(args);

// Recommended: register builder-composed settings
builder.Services.AddSingleton<ICommanderSettings>(settings);

builder.Services.UseSyrx(_ => { });

// Use SQL Client provider (for SQL Server)
builder.Services.UseDatabaseConnector(() => System.Data.SqlClient.SqlClientFactory.Instance);

// Register your repository
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// ... rest of your setup
```

Optional file-based registration:

```csharp
var configBuilder = new ConfigurationBuilder();

builder.Services.UseSyrx(config =>
  config.UseFile("syrx.json", configBuilder));
```

---

## Step 4: Inject and Use

In your controllers or services:

```csharp
// Some controller or service
public class UsersController
{
    private readonly IUserRepository _users;
    
    public UsersController(IUserRepository users)
    {
        _users = users;
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> RetrieveUser(int id, CancellationToken cancellationToken = default)
    {
      var user = await _users.RetrieveAsync(id, cancellationToken);
        if (user == null)
            return NotFound();
        return Ok(user);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user, CancellationToken cancellationToken = default)
    {
        var success = await _users.CreateAsync(user, cancellationToken);
        if (!success)
          return BadRequest("Failed to create user");
        return Created($"/users/{user.Id}", user);
    }
}
```

---

## How It Works: The Magic of Method Names

**Syrx resolves repository methods to configuration keys automatically:**

```csharp
public async Task<IEnumerable<User>> RetrieveAllAsync(
    int page = 1,
    int size = 100,
    CancellationToken cancellationToken = default)
```

For this repository method, Syrx looks for:
```
Namespace: MyApp.Data
Type: UserRepository
Method: RetrieveAllAsync
```

And loads the corresponding command from configuration:
```json
"namespace": "MyApp.Data",
"types": [
  {
    "name": "UserRepository",
    "commands": {
      "RetrieveAllAsync": { "commandText": "..." }
    }
  }
]
```

Syrx uses the `[CallerMemberName]` attribute automatically for non-overloaded repository methods — no annotations or metaprogramming required.

Prefer distinct method names such as `RetrieveAllAsync` for list queries. If you intentionally overload methods, pass `method:` directly to `_commander.QueryAsync<>` or `_commander.ExecuteAsync<>` so each overload resolves to a distinct command key.

---

## Common Patterns

### Multi-Mapping Example

For queries that join multiple tables:

```csharp
// Repository method
public async Task<IEnumerable<UserWithProfile>> RetrieveWithProfilesAsync(CancellationToken cancellationToken = default)
{
    return await _commander.QueryAsync<User, Profile, UserWithProfile>(
        (user, profile) => new UserWithProfile 
        { 
            User = user, 
            Profile = profile 
    },
    cancellationToken: cancellationToken
    );
}

// Configuration
{
  "RetrieveWithProfilesAsync": {
    "commandText": "SELECT * FROM Users u JOIN Profiles p ON u.Id = p.UserId WHERE u.IsActive = 1",
    "connectionAlias": "Default",
    "split": "Id"  // Dapper split column for multi-mapping
  }
}
```

### Multiple Result Sets

For stored procedures that return multiple result sets:

```csharp
public async Task<(IEnumerable<User>, IEnumerable<Profile>)> RetrieveDataAsync(CancellationToken cancellationToken = default)
{
    return await _commander.QueryAsync<User, IEnumerable<Profile>, (IEnumerable<User>, IEnumerable<Profile>)>(
    (users, profiles) => (users, profiles),
    cancellationToken: cancellationToken
    );
}
```

### Stored Procedures

Use stored procedures as easily as ad-hoc SQL:

```csharp
// Configuration
{
  "RetrieveByStatus": {
    "commandText": "sp_GetUsersByStatus",
    "commandType": "StoredProcedure",  // Set to StoredProcedure
    "connectionAlias": "Default"
  }
}
```

---

## Configuration for Multiple Databases

When your application uses multiple databases:

```csharp
builder.Services.UseSyrx(config =>
{
    config
        .AddConnectionString("Primary", "Server=primary;Database=Main;...")
        .AddConnectionString("Reporting", "Server=reporting;Database=Reports;...")
        .UseFile("syrx.json", settings =>
        {
            // Additional configuration
        });
});
```

Then in your configuration, reference the appropriate alias:

```json
{
  "commands": {
    "RetrieveFromPrimary": {
      "commandText": "SELECT ...",
      "connectionAlias": "Primary"
    },
    "RetrieveFromReporting": {
      "commandText": "SELECT ...",
      "connectionAlias": "Reporting"
    }
  }
}
```

---

## Next Steps

- **Read about configuration** → [Configuration Overview](configuration/index.md)
- **Understand command resolution** → [Command Resolution](architecture/command-resolution.md)  
- **Learn about advanced patterns** → [Architecture Overview](architecture/index.md)
- **Explore the full API** → [Reference Documentation](projects/index.md)
- **See configuration examples** → [Configuration Examples](configuration/examples.md)

---

## Troubleshooting

### "Command not found" Exception

**Cause**: Configuration doesn't match repository/method name exactly (case-sensitive)

**Solution**: Verify:
1. Namespace matches exactly
2. Type name matches exactly
3. Method name matches exactly
4. Configuration payload is valid

```csharp
// Code:
namespace MyApp.Data { 
    public class UserRepository { 
        public async Task RetrieveAllAsync(int page = 1, int size = 100, CancellationToken cancellationToken = default) { }
    } 
}

// Must match in config:
{
  "namespace": "MyApp.Data",     // Must match exactly
  "types": [{
    "name": "UserRepository",    // Must match exactly
    "commands": {
      "RetrieveAllAsync": { ... } // Method name must match exactly
    }
  }]
}
```

### Connection String Issues

**Cause**: Connection alias not found

**Solution**: Verify connection alias exists and is referenced correctly:

```json
{
  "connections": [{
    "alias": "Default",              // Define the alias
    "connectionString": "..."
  }],
  "namespaces": [{
    "commands": {
      "RetrieveData": {
        "connectionAlias": "Default" // Reference it here
      }
    }
  }]
}
```

### Injection Issues

**Cause**: Framework not registered or wrong provider factory

**Solution**: Ensure all registrations are in place:

```csharp
// MUST do both:
builder.Services.UseSyrx(...);
builder.Services.UseDatabaseConnector(() => SqlClientFactory.Instance);

// Register repository:
builder.Services.AddScoped<IUserRepository, UserRepository>();
```

---

## Performance Tips

1. **Reuse configuration**: Load configuration once at startup; immutable thereafter
2. **Use async**: Async methods are preferred and provide better scalability
3. **Connection pooling**: Syrx leverages ADO.NET connection pooling automatically
4. **Command caching**: Command resolution is cached automatically; no warmup needed
5. **Buffered queries**: Default Dapper flag; disable only if needed for very large result sets

---

**Ready to start?** [Create your first repository!](#step-1-create-your-repository)

For more detailed examples and patterns, see [Configuration Examples](configuration/examples.md).



