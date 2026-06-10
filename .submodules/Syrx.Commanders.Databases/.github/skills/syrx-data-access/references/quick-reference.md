# Syrx Data Access Quick Reference

Use this file as the default reference when repository-specific docs are missing.

## Core Objectives

- Keep repositories focused on persistence mapping.
- Use Syrx ICommander<TRepository> as the repository execution boundary.
- Use explicit, parameterized SQL only.
- Keep command mapping deterministic (method name or explicit command mapping).
- Keep tests deterministic with mocked commander behavior.

## Minimal Repository Pattern

```csharp
public interface IUserRepository
{
    Task<User?> RetrieveAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> SaveAsync(User user, CancellationToken cancellationToken = default);
}

public sealed class UserRepository : IUserRepository
{
    private readonly ICommander<UserRepository> _commander;

    public UserRepository(ICommander<UserRepository> commander)
    {
        _commander = commander;
    }

    public async Task<User?> RetrieveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Throw<ArgumentOutOfRangeException>(id != Guid.Empty, nameof(id));

        var rows = await _commander.QueryAsync<User>(new { id }, cancellationToken)
            ?? Enumerable.Empty<User>();

        return rows.FirstOrDefault();
    }

    public Task<bool> SaveAsync(User user, CancellationToken cancellationToken = default)
    {
        Throw<ArgumentNullException>(user != null, nameof(user));
        return _commander.ExecuteAsync(user, cancellationToken);
    }
}
```

## SQL Rules

- No SELECT *.
- No SQL string concatenation from user input.
- Match parameter names to model/anonymous object names.
- Use paging for large list queries.

## Configuration Modes

- Mode A (configuration-first): builder/JSON/XML command mapping by namespace.type.method.
- Mode B (command constant-first): CommandStrings constants with installer mapping.

Pick one mode per bounded context and apply consistently.

## Test Rules

- Unit test repositories with mocked ICommander<TRepository>.
- Do not hit a live database in unit tests.
- Keep at least one integration path for command configuration verification.
