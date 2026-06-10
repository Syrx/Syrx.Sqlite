# Syrx.Commanders.Databases.Connectors.Extensions

## Summary
DI and builder integration package for wiring provider factories, command readers, connectors, and commanders.

## Key Types and Interfaces
- `DatabaseConnectorExtensions.UseConnector(...)`: end-to-end Syrx builder registration helper.
- `ServiceCollectionExtensions.AddProvider(...)`: registers `Func<DbProviderFactory>`.
- `ServiceCollectionExtensions.AddDatabaseConnector<TService, TImplementation>(...)`: registers connector implementation.

## Usage Notes
- `UseConnector(...)` builds `CommanderSettings` from `Action<CommanderSettingsBuilder>` and registers:
  - provider delegate;
  - `ICommanderSettings` singleton;
  - command reader;
  - connector;
  - commander.
- `AddProvider(...)` enforces non-null delegate and uses `TryAddToServiceCollection` semantics.
- `AddDatabaseConnector<TService, TImplementation>(...)` supports selectable lifetime (default transient).

## Thread Safety
- Extension methods are stateless and thread-safe.
- Thread-safety of resulting runtime depends on chosen service lifetime and implementation types.

## Security Considerations
- Provider delegate should be trusted and deterministic.
- Keep settings builder inputs controlled; connection strings and SQL remain externally supplied configuration.
- Prefer explicit least-privilege connection principals in configured strings.

## Performance Notes
- Registration methods run at startup and have negligible runtime overhead.
- Singleton registration path in `UseConnector(...)` reduces repeated object construction.

## See Also
- [Syrx.Commanders.Databases.Connectors](./connectors.md)
- [Syrx.Commanders.Databases.Settings.Extensions](./settings-extensions.md)
- [Syrx.Commanders.Databases.Extensions](./extensions.md)
