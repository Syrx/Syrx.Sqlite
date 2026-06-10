---
name: api-design
description: Use when designing external API integrations with resilient client patterns, DTO contracts, and clear integration boundaries.
---

# API Design Skill

## Mandatory Inputs

- Language
- Endpoint or API surface
- At least one HTTP verb or operation

Optional inputs:

- Request and response DTOs
- Authentication model
- Resilience requirements such as retry, timeout, circuit breaker, bulkhead, or throttling
- Required tests

## Design Model

Use a three-layer pattern when it adds value:

1. Service: raw transport and protocol handling
2. Manager: orchestration and abstraction
3. Resilience: policy execution and failure handling

## .NET-Specific Rules

- Use `HttpClient` correctly through DI.
- Prefer records for immutable DTOs.
- Validate all public entry points.
- Map errors explicitly instead of leaking transport exceptions blindly.
- Use Polly or the team's current resilience stack when resilience is required.

## Security & Reliability

- Never hardcode secrets.
- Prefer HTTPS.
- Validate outbound URLs and configuration.
- Define timeout, retry, and failure behavior deliberately.