# Syrx.Commanders.Databases.Builders

## Summary
Schema modeling package for constructing in-memory database, table, and field definitions through fluent builders.

## Key Types and Interfaces
- `Database`: immutable-style model with `Name` and `IEnumerable<Table>`.
- `Table`: model with `Schema`, `Name`, and `IEnumerable<Field>`.
- `Field`: model with `Name`, `SqlDbType`, nullable flag, and optional width.
- `DatabaseOptions`, `TableOptions`, `FieldOptions`: fluent builders.
- `DatabaseOptionsBuilderExtensions`, `TableOptionsBuilderExtensions`, `FieldOptionsBuilderExtensions`: static build helpers.

## Usage Notes
- Build a database model by chaining:
  - `DatabaseOptions.WithName(...)`
  - `DatabaseOptions.AddTable(...)`
  - `TableOptions.AddField(...)`
- Validation is guard-based:
  - names cannot be null/blank;
  - collections must contain at least one table/field before `Build()`.
- `Table` defaults schema to `dbo` when omitted or whitespace.
- `FieldOptions.IsIdentity(...)` currently stores a flag internally but does not surface it on `Field`.

## Thread Safety
- Built models (`Database`, `Table`, `Field`) expose getters only, but they retain enumerable references supplied at construction.
- Builder classes use mutable dictionaries and fields; treat builder instances as not thread-safe.

## Security Considerations
- Package does not execute SQL or open connections.
- Guard checks prevent invalid names and empty definitions from propagating.
- If used for SQL generation in downstream code, enforce identifier allow-lists there.

## Performance Notes
- Lightweight object construction with minimal allocations.
- Uses `Dictionary` for name-based table/field accumulation.
- Build helpers avoid reflection and runtime code generation.

## See Also
- [Syrx.Commanders.Databases.Settings.Extensions](./settings-extensions.md)
- [Syrx.Commanders.Databases.Settings](./settings.md)
- [Configuration Overview](../configuration/index.md)
