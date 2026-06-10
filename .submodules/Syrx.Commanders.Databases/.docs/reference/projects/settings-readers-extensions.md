# Syrx.Commanders.Databases.Settings.Readers.Extensions

## Summary
DI registration package for wiring the default database command reader implementation.

## Key Types and Interfaces
- `ServiceCollectionExtensions.AddReader(this IServiceCollection, ServiceLifetime lifetime = ServiceLifetime.Transient)`.
- Registers `IDatabaseCommandReader` -> `DatabaseCommandReader`.

## Usage Notes
- Call `AddReader(...)` during service registration to enable command lookup resolution.
- Lifetime is configurable; default is transient.
- In full Syrx setup, this method is often invoked through higher-level connector registration flows.

## Thread Safety
- Extension method itself is stateless and thread-safe.
- Reader runtime safety depends on the selected lifetime and immutable command settings assumptions.

## Security Considerations
- No direct data access; only service registration.
- Registration should be part of controlled startup composition.

## Performance Notes
- Startup-only cost.
- Singleton reader registration can reduce repeated indexing cost if settings are stable.

## See Also
- [Syrx.Commanders.Databases.Settings.Readers](./settings-readers.md)
- [Syrx.Commanders.Databases.Connectors.Extensions](./connectors-extensions.md)
