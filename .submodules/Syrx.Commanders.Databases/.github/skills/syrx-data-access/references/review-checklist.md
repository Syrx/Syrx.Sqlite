# Syrx Repository Review Checklist

Use this checklist when reviewing implementation quality.

## Correctness

- Repository methods map to the intended command key or explicit command string.
- Parameter names in SQL and method payloads match.
- Query methods return stable shapes for empty results.
- Write methods return clear success/failure semantics.

## Security

- SQL is parameterized.
- No user-controlled SQL fragments.
- No secrets or connection strings hardcoded.
- Error handling does not leak SQL internals.

## Performance

- List queries are bounded (paging or explicit limits).
- No N+1 loops through repository methods.
- Multi-mapping or multi-result usage is justified by fewer round trips.

## Design

- Repository contains persistence logic only.
- Business rules stay in domain/application layers.
- Command configuration source is consistent in the bounded context.

## Testing

- Unit tests mock ICommander<TRepository>.
- Guard behavior is tested for invalid inputs.
- Integration tests validate command configuration and SQL execution paths.
