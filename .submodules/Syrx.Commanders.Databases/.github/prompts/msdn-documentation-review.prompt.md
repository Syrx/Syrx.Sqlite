---
name: msdn-documentation-review
description: 'Prompt to run a full MSDN-style documentation review, update stale docs, create missing docs, and validate links across the workspace.'
---

# MSDN Documentation Review Prompt

Route this request to `orchestrator`.

Use the `msdn-documentation` skill as the primary workflow.

## Goal

Review all existing documentation, detect drift against source code, update stale content, create missing pages, and return a trustworthy MSDN-style documentation set for the full workspace.

## Required Execution Model

- Use `planning-and-research` to create an inventory-first documentation plan.
- Use parallel exploration for code understanding by path partitions.
- Use `Explore` for read-only parallel scans.
- Use `architecture-and-ddd` where boundary or domain terminology is ambiguous.
- Use `csharp-engineering` for deep .NET member and behavior interpretation.
- Use `critical-thinking` to challenge assumptions before finalizing claims.

## Coverage Requirements

- Attempt to document all files in the workspace.
- If a file is excluded, record the reason in coverage reporting.
- Mirror documentation structure to repository structure.
- Prefer updating existing docs in place over replacing files wholesale.

## Output Requirements

Generate or update documentation under `/.docs/reference/` with at least:

- `index.md`
- `code-structure.md`
- `coverage-report.md`
- Directory-level indexes reflecting source layout
- File/module/type pages as applicable

## Quality Gates

- Every technical claim must be traceable to code or existing verified docs.
- No speculative statements.
- All internal links must point to existing files.
- Regenerate index and coverage report after any file additions or moves.
- Explicitly note unresolved evidence gaps.

## Final Report Back to User

Provide a concise final report containing:

- What was created
- What was updated
- What was excluded and why
- Coverage percentage
- Link validation status
- Remaining risks or follow-up items
