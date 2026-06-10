---
name: ms-sql-dba
applyTo: "**/*.sql"
description: 'SQL Server standards for T-SQL files: parameterization, naming, safety, and compatibility rules.'
---

# SQL Server Standards

## Safety Rules
- Never use `SELECT *` in production queries; enumerate columns explicitly.
- Always use explicit transactions for multi-statement writes.
- Wrap destructive operations (`DELETE`, `UPDATE`, `TRUNCATE`) in a transaction with a rollback-capable test condition.
- Never use `DROP` or `TRUNCATE` in migration scripts without a guarded existence check.

## Parameterization
- All user-supplied values must be passed as parameters, never interpolated into query strings.
- Use `@ParameterName` syntax for all T-SQL parameters.
- Name parameters after the data they carry, not the column they map to when these differ.

## Naming Conventions
- Tables: `PascalCase`, plural (`Orders`, `Customers`).
- Columns: `PascalCase`, singular noun (`OrderId`, `CreatedAt`).
- Stored procedures: `usp_VerbNoun` (e.g., `usp_GetOrderById`).
- Indexes: `IX_TableName_ColumnName(s)`.
- Primary keys: `PK_TableName`.
- Foreign keys: `FK_ChildTable_ParentTable`.

## Query Patterns
- Prefer `EXISTS` over `COUNT(*)` for existence checks.
- Use `NOLOCK` hints only when explicitly justified and documented; never as a default.
- Avoid cursors; prefer set-based operations.
- Use `TOP` with `ORDER BY` when pagination is not implemented; never use `TOP` alone without `ORDER BY` for deterministic results.

## Compatibility
- Target SQL Server 2019 or later unless the project explicitly targets an earlier version.
- Avoid deprecated features. Flag usage of `SET ROWCOUNT`, `\!= `, text/ntext/image types, and non-ANSI joins.
- Use `TRY_CAST` and `TRY_CONVERT` over `CAST`/`CONVERT` when input is untrusted.

## Security
- Grant least-privilege: use schema-scoped permissions, not `db_datareader`/`db_datawriter` unless justified.
- Never store plaintext passwords or secrets in SQL scripts or tables.
- Audit sensitive column access (PII, financial data) via triggers or temporal tables where required.
