# Syrx.Commanders.Databases.Extensions

## Summary
Core DI extension package for registering database commander services.

## Key Types and Interfaces
- `ServiceCollectionExtensions.AddDatabaseCommander(...)`: registers open generic `ICommander<>` to `DatabaseCommander<>`.

## Usage Notes
- Call `AddDatabaseCommander(...)` on `IServiceCollection` to register commander implementation.
- Default lifetime is transient; you can provide `ServiceLifetime` explicitly.
- In complete setups, this registration is commonly invoked indirectly through connector extension workflows.

## Thread Safety
- Extension methods are stateless and safe.
- `DatabaseCommander<TRepository>` runtime thread-safety is documented in the core package page.

## Security Considerations
- DI registration itself has no data access behavior.
- Security posture depends on configured connectors, settings, and SQL content.

## Performance Notes
- Startup-only registration cost.
- Open generic registration avoids repetitive per-repository registration code.

## See Also
- [Syrx.Commanders.Databases](./commanders-databases.md)
- [Syrx.Commanders.Databases.Connectors.Extensions](./connectors-extensions.md)
