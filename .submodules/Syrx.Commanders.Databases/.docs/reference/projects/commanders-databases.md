# Syrx.Commanders.Databases

## Summary
Core runtime package that implements `ICommander<TRepository>` using Dapper, command settings resolution, and connector-provided ADO.NET connections.

## Key Types and Interfaces
- `DatabaseCommander<TRepository>`: command execution and query orchestration.
- `ICommander<TRepository>`: service contract implemented by the commander.
- `IDatabaseCommandReader`: resolves `CommandSetting` by repository type and method key.
- `IDatabaseConnector`: creates `IDbConnection` from `CommandSetting`.
- `CommandSetting`: runtime command metadata used to build Dapper `CommandDefinition`.

## Usage Notes
- Register through DI helpers in sibling packages, then inject `ICommander<TRepository>` into repositories.
- Command resolution key is `${typeof(TRepository).FullName}.${method}` where `method` is typically provided by `CallerMemberName`.
- `Execute` and `ExecuteAsync` wrap writes in a transaction (`BeginTransaction(setting.IsolationLevel)`), commit on success, and rollback on failure.
- Query APIs are split across partial class files:
  - Single-result and multi-map query overloads.
  - Multi-result-set overloads using Dapper `GridReader`.
  - Async equivalents with cancellation token support.
- `Execute(Func<TResult>)` and `ExecuteAsync(Func<...>)` provide transaction-scope wrappers for custom mapped logic.

## Thread Safety
- `DatabaseCommander<TRepository>` caches command settings in `ConcurrentDictionary<string, CommandSetting>`.
- Static reflection caches for multi-result operations use `ConcurrentDictionary<Type, MethodInfo>`.
- The commander is thread-safe regardless of service lifetime; default Add* registrations in this repo are transient, while some Use* extension paths register singleton lifetimes.
- Connections and transactions are per-operation and not shared.

## Security Considerations
- SQL is configuration-driven; no SQL sanitization is added by the commander. Use parameterized SQL in command text.
- Input validation is enforced for key APIs via guards (`Throw<...>`), reducing null/invalid call paths.
- Failure tracing logs repository/method/alias metadata. Avoid placing secrets in aliases or method names.

## Performance Notes
- Command lookup is O(1) after first call due to `_commandCache.GetOrAdd`.
- Precomputed `_typeFullName` avoids repeated reflection on each command resolution.
- Reusable reflection method caches reduce overhead for high-arity multi-result mapping.
- Async open path uses `DbConnection.OpenAsync` when available, otherwise guarded sync fallback.

## See Also
- [Syrx.Commanders.Databases.Settings.Readers](./settings-readers.md)
- [Syrx.Commanders.Databases.Connectors](./connectors.md)
- [Syrx.Commanders.Databases.Extensions](./extensions.md)
- [Thread Safety](../architecture/thread-safety.md)
- [Transaction Handling](../architecture/transaction-handling.md)
