# Syrx.Commanders.Databases.Settings.Readers

Internal command configuration reader for the Syrx database framework.

## Overview

`IDatabaseCommandReader` is an **internal component** used by `DatabaseCommander<TRepository>` to resolve command settings from configuration. It is **never used directly** in application code - developers work with `ICommander<TRepository>` instead.

## Key Points

- ⚠️ **Internal Component**: Used internally by `DatabaseCommander<TRepository>`
- ✅ **Repository Pattern**: Your repositories use `ICommander<TRepository>`, not this interface
- 🔄 **Automatic Resolution**: Method names are automatically mapped to command settings via `[CallerMemberName]`
- 🏗️ **Framework Architecture**: Part of the internal plumbing, not user-facing API

## Installation

```bash
dotnet add package Syrx.Commanders.Databases.Settings.Readers
```

**Package Manager**
```bash
Install-Package Syrx.Commanders.Databases.Settings.Readers
```

**PackageReference**
```xml
<PackageReference Include="Syrx.Commanders.Databases.Settings.Readers" Version="3.0.0" />
```

## Architecture Overview

The reader sits in the internal architecture between `DatabaseCommander` and the configuration system:

```
Repository Class
     ↓ (uses)
ICommander<TRepository>
     ↓ (implemented by)
DatabaseCommander<TRepository>
     ↓ (uses internally)
IDatabaseCommandReader ← You are here
     ↓ (reads from)
Configuration Settings
```

## Interface Definition

```csharp
public interface IDatabaseCommandReader : ICommandReader<CommandSetting>
{
    // Inherits: CommandSetting GetCommand(Type type, string key)
}
```

## Implementation

```csharp
public class DatabaseCommandReader : IDatabaseCommandReader
{
    private readonly ICommanderSettings _settings;
        
    public DatabaseCommandReader(ICommanderSettings settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }
        
    public CommandSetting GetCommand(Type type, string key)
    {
        return _settings.Namespaces
            .SelectMany(x => x.Types.Where(y => y.Name == type.FullName))
            .SelectMany(z => z.Commands)
            .SingleOrDefault(f => f.Key == key).Value;
    }
}
```

## How It Works Internally

When you call a repository method, here's what happens:

1. **Repository Call**: `await _commander.QueryAsync<User>(new { id })`
2. **DatabaseCommander**: Receives the call with `[CallerMemberName]` method name
3. **Command Resolution**: `_reader.GetCommand(typeof(Repository), "MethodName")`
4. **Settings Lookup**: Finds the configured SQL command for this repository method
5. **Execution**: Creates `CommandDefinition` and executes via Dapper

## Correct Usage Pattern

❌ **WRONG** - Never use `IDatabaseCommandReader` directly:
```csharp
public class UserRepository
{
    private readonly IDatabaseCommandReader _reader; // ❌ NO!
    
    public UserRepository(IDatabaseCommandReader reader) // ❌ NO!
    {
        _reader = reader;
    }
}
```

✅ **CORRECT** - Use `ICommander<TRepository>`:
```csharp
public class UserRepository
{
    private readonly ICommander<UserRepository> _commander; // ✅ YES!
    
    public UserRepository(ICommander<UserRepository> commander) // ✅ YES!
    {
        _commander = commander;
    }
    
    public async Task<User> RetrieveAsync(int id, CancellationToken cancellationToken = default)
        => (await _commander.QueryAsync<User>(new { id }, cancellationToken)).FirstOrDefault();
        
    public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default)
        => await _commander.ExecuteAsync(user, cancellationToken) ? user : default;
}
```

## Configuration Example

The reader resolves method names to configured commands:

```csharp
services.UseSyrx(builder => builder
    .UseSqlServer(sqlServer => sqlServer
        .AddCommand<UserRepository>(commands => commands
            .ForMethod(nameof(UserRepository.RetrieveAsync), command => command
                .UseConnectionAlias("DefaultConnection")
                .UseCommandText("SELECT * FROM Users WHERE Id = @id"))
            .ForMethod(nameof(UserRepository.CreateUserAsync), command => command
                .UseConnectionAlias("DefaultConnection")
                .UseCommandText("INSERT INTO Users (Name, Email) VALUES (@Name, @Email)")))));
```

## Related Packages

- **[Syrx.Commanders.Databases](../Syrx.Commanders.Databases)** - Contains `DatabaseCommander<T>` that uses this reader
- **[Syrx.Commanders.Databases.Settings](../Syrx.Commanders.Databases.Settings)** - Core settings definitions
- **[Syrx](../../.submodules/Syrx/src/Syrx)** - Contains `ICommander<T>` interface for repositories

## Dependencies

This package depends on:
- **Syrx.Commanders.Databases.Settings** - Core settings definitions
- **Syrx.Readers** - Base reader interfaces

## License

This project is licensed under the [MIT License](https://github.com/Syrx/Syrx/blob/main/LICENSE).

## Credits

Built as part of the Syrx data access framework to provide internal command resolution capabilities.



