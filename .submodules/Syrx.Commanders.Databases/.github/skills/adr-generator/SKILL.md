---
name: adr-generator
description: Use when documenting architectural decisions in .docs/adr with explicit rationale, alternatives, and consequences.
---

# ADR Generator Skill

## Singular Purpose

Author Architectural Decision Records. Nothing else.

## Required Inputs

Collect all inputs before writing. Block generation and ask if any are missing.

| Input | Description |
|---|---|
| `title` | Short decision title |
| `context` | Drivers, constraints, and background |
| `decision` | The chosen option, stated concisely |
| `alternatives` | At least two alternatives including "Do nothing" |
| `stakeholders` | Owners or affected teams |

## Generation Flow

```
Gather Inputs → Validate → Determine Next Number → Draft Sections →
Enumerate Consequences → Document Alternatives with Rejection Rationale →
Add Implementation Notes → Inject References → Quality Gate → Save File
```

## Numbering

Scan `/.docs/adr/` for the highest existing `adr-NNNN-` prefix. Increment by one. Use zero-padded four-digit format.

## Output Rules

- Save in `.docs/adr/`.
- Filename: `adr-NNNN-title-slug.md` (lowercase, hyphens, no special chars).
- Include all mandatory sections in order.

## ADR Template

```markdown
---
title: "ADR-NNNN: <Decision Title>"
status: "Proposed"
date: "<YYYY-MM-DD>"
authors: "<Owner or Team>"
tags: ["architecture", "decision"]
supersedes: ""
superseded_by: ""
---

## Context

<Describe the drivers, constraints, and background that make this decision necessary.>

## Decision

<State the chosen option clearly and concisely.>

## Consequences

### Positive
- **POS-001**: <Benefit>
- **POS-002**: <Benefit>

### Negative
- **NEG-001**: <Trade-off or risk>
- **NEG-002**: <Trade-off or risk>

## Alternatives

### <Alternative 1 Name>
<Description of the alternative and explicit rejection rationale referencing constraints.>

### <Alternative 2 Name>
<Description and rejection rationale.>

### Do Nothing
<What happens if no decision is made and why that is unacceptable.>

## Implementation Notes

<Practical steps, monitoring requirements, rollback strategy, and security considerations.>

## References

<Links, tickets, related ADRs, or external documentation.>
```

## Quality Gate

Do not save the ADR until all checklist items pass:

- [ ] Numbering is sequential and unique.
- [ ] At least two alternatives with explicit rejection rationale tied to stated constraints.
- [ ] At least two positive and two negative consequences with IDs.
- [ ] Implementation Notes include monitoring and rollback steps.
- [ ] Security considerations stated explicitly.
- [ ] Event or domain names follow `<Aggregate><PastTense>` where applicable.
- [ ] "Do nothing" alternative is present.

## Consequence ID Format

- Positive outcomes: `POS-001`, `POS-002`, ...
- Negative outcomes: `NEG-001`, `NEG-002`, ...

## Output Quality

- Precise, unambiguous language.
- Both benefits and trade-offs represented honestly.
- No vague consequences ("improved maintainability" must state what specifically changes).