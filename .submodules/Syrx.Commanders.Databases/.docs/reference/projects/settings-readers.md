# Syrx.Commanders.Databases.Settings.Readers

## Summary
Command reader package that indexes and resolves `CommandSetting` entries for repository type and method lookups.

## Key Types and Interfaces
- `IDatabaseCommandReader`: command reader contract.
- `DatabaseCommandReader`: default implementation backed by a lookup dictionary.

## Usage Notes
- Constructor consumes `ICommanderSettings` and eagerly indexes commands using key pattern:
  - `${type.Name}.${commandKey}` where `type.Name` is the configured repository type name.
- Duplicate keys across settings throw an argument exception during reader construction.
- `GetCommand(Type type, string key)` resolves using `${type.FullName}.${key}` and throws if missing.

## Thread Safety
- Internal lookup storage is `Dictionary<string, CommandSetting>` populated only in constructor.
- After construction, reads are concurrent-safe for practical usage because no further writes occur.
- Register as singleton when paired with immutable/read-only settings.

## Security Considerations
- Reader performs key validation and fails closed when command entries are missing.
- It does not inspect or sanitize SQL text; secure command content must be enforced upstream.

## Performance Notes
- Eager indexing shifts cost to startup and makes runtime command resolution O(1).
- Avoids repeated traversal through namespace/type hierarchy for every command invocation.

## See Also
- [Syrx.Commanders.Databases.Settings](./settings.md)
- [Syrx.Commanders.Databases.Settings.Readers.Extensions](./settings-readers-extensions.md)
- [Command Resolution](../architecture/command-resolution.md)
