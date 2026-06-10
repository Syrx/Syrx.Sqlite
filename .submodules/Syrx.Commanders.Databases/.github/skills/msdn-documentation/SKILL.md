---
name: msdn-documentation
description: Use when generating or refreshing MSDN-style repository documentation with code-verified content and link integrity checks.
---

# MSDN Documentation Skill

## Role

Use this skill to generate and maintain rich, developer-trustworthy documentation that mirrors the repository structure and stays aligned with real code behavior.

This skill is documentation-first and evidence-first:

- Document what is verifiably true from source code and existing repository artifacts.
- Flag unknowns explicitly instead of guessing.
- Keep links valid and navigable.
- Prefer incremental updates over full rewrites when documents already exist.

## Mandatory Inputs

- Documentation scope (default: entire workspace).
- Documentation root output path (default: `/.docs/reference/`).
- Preferred audience (default: developers and maintainers).
- Language/ecosystem hints when available (for API idioms and examples).

If required input is missing, ask focused questions before writing.

## Companion Agent and Skill Orchestration

Use these collaborators intentionally:

- `orchestrator` for phase routing, boundary control, and parallel sub-task fan-out.
- `planning-and-research` for inventory, evidence gathering, and phased plan creation.
- `Explore` for fast parallel codebase analysis by path segment.
- `architecture-and-ddd` for domain boundaries, aggregate context, and ADR-aware rationale.
- `csharp-engineering` for detailed API/member interpretation in .NET projects.
- `task-research` skill for deep evidence collection and source-backed findings.
- `critical-thinking` skill for ambiguity pressure tests before publication.
- `api-design` skill when documenting external integrations and resilience policies.

### Parallelization Policy

When scope is workspace-wide, split by top-level directory and run discovery in parallel where safe:

1. Build canonical file inventory.
2. Partition inventory by top-level folder and by artifact type (code, config, scripts, docs).
3. Analyze partitions in parallel via `Explore` or equivalent read-only workflows.
4. Merge findings into one documentation graph.
5. Run one final consistency and link audit pass.

## Output Contract

Produce an MSDN-style documentation set under the selected root (default `/.docs/reference/`) with this baseline:

- `index.md` - repository documentation home page.
- `code-structure.md` - source tree mirror and navigation map.
- `coverage-report.md` - documented vs undocumented file matrix.
- One documentation page for each significant source file, type, or module.
- Directory-level landing pages that mirror the code hierarchy.

Use the provided templates:

- [MSDN API page template](./references/msdn-api-page-template.md)
- [Documentation index template](./references/documentation-index-template.md)
- [Coverage report template](./references/coverage-report-template.md)

## MSDN-Style Page Requirements

Each generated reference page should include applicable sections:

1. Summary
2. Package/Namespace/Module
3. Syntax or Signature
4. Parameters
5. Return Value
6. Exceptions and Error Conditions
7. Remarks
8. Thread Safety and Concurrency Notes
9. Security Considerations
10. Performance Notes
11. Examples (basic and advanced where feasible)
12. See Also

Use "Not observed in code" when data is unavailable.

## Reliability and Trust Gates

Before finalizing documentation, enforce all gates:

- Evidence Gate: Every technical claim maps to actual code or existing docs.
- Drift Gate: Remove or correct stale statements contradicted by code.
- Coverage Gate: Attempt to document every file in scope; explicitly justify exclusions. Run [`Get-DocumentationMetrics.ps1`](./Get-DocumentationMetrics.ps1) to measure XML doc coverage and README coverage objectively.
- Link Gate: Validate that every internal link target exists.
- Consistency Gate: Terminology is aligned with code symbols and established naming.

### Documentation Metrics Script

[`Get-DocumentationMetrics.ps1`](./Get-DocumentationMetrics.ps1) measures documentation quality across the solution:

- **XML documentation coverage**: percentage of public C# declarations with `///` XML doc comments.
- **README coverage**: percentage of `src/` projects with a `README.md`.
- **Composite score**: weighted result from both metrics.

Usage:

```powershell
# Check current coverage (default minimum: 80)
./.github/skills/msdn-documentation/Get-DocumentationMetrics.ps1

# Enforce a minimum score and export metrics as JSON
./.github/skills/msdn-documentation/Get-DocumentationMetrics.ps1 -MinimumScore 85 -EnforceMinimum -OutputJsonPath ./.docs/reports/doc-metrics.json
```

The CI/CD pipeline runs this script with `-EnforceMinimum` on every push and pull request. Documentation work is blocked from merging if the composite score falls below the configured threshold.

## Workspace Coverage Rules

Default behavior is full-workspace attempt:

- Include source files, scripts, configuration, and infrastructure artifacts.
- Exclude generated folders only with explicit rationale (for example, build outputs).
- For binary or opaque artifacts, create short descriptor entries rather than fabricating internals.

Maintain a documented exclusion list in `coverage-report.md`.

## Link Integrity Requirements

- Use relative links.
- Ensure every linked document exists at generation time.
- Avoid dead references to planned-but-missing files.
- Re-run link validation after any rename or move.

If links cannot be validated automatically, run a deterministic manual check from the generated index and record unresolved items.

## Review and Update Mode

When invoked in review mode:

1. Inventory current documentation.
2. Identify missing docs and stale docs.
3. Update existing files in place where possible.
4. Create missing pages needed to satisfy coverage.
5. Regenerate index and coverage report.
6. Validate links and record final status.

## Writing Standards

- Markdown only, clear heading hierarchy, flat lists for scanability.
- Keep style concise but technically complete.
- Prefer exact symbol names and file-backed references.
- Never invent behavior, defaults, ranges, or guarantees.
- Use consistent requirement identifiers when writing specification-like sections.

## Completion Checklist

Documentation work is complete only when:

- `index.md`, `code-structure.md`, and `coverage-report.md` are present.
- Documentation tree reflects repository structure.
- Coverage report explains included and excluded files.
- Internal links resolve to existing docs.
- No unresolved evidence gaps remain without explicit note.
- [`Get-DocumentationMetrics.ps1`](./Get-DocumentationMetrics.ps1) passes at the configured minimum score (default: 80; CI enforces 85).
