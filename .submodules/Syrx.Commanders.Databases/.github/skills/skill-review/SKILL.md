---
name: skill-review
description: Use when evaluating one or more workspace skills against mandatory quality standards, tracking review history, and producing remediation recommendations.
---

# Skill Review

## Singular Purpose

Evaluate skills against defined quality standards and produce review outcomes that determine whether a skill passes, requires update, or enters conflict resolution.

This skill has one purpose only: skill quality review and follow-up governance.

## Normative Language

- MUST: Mandatory requirement. Failure means the reviewed skill fails.
- SHOULD: Advisory requirement. Failure does not auto-fail, but requires recommendation and tracking.

## Review Standards

Use these standards exactly:

| ID | Standard | Type | Pass Criteria | Failure Action |
|---|---|---|---|---|
| SKR-M1 | Singular purpose | MUST | Skill scope maps to one objective only. | Mark as failed and recommend scope split or refocus. |
| SKR-M2 | Valid format | MUST | Valid YAML front matter and valid Markdown structure for Copilot skill loading. | Mark as failed and provide exact formatting fix. |
| SKR-M3 | Triggers | MUST | Clear discovery triggers in description and body with concrete use conditions. | Mark as failed and provide trigger rewrite guidance. |
| SKR-S1 | Assets | SHOULD | Skill has concrete references or reusable assets for execution support. | Record advisory finding and recommend assets. |
| SKR-S2 | No conflict | SHOULD NOT conflict | No harmful overlap or contradictory behavior with other skills. | Start conflict workflow and document resolution plan. |

## Trigger Conditions

Invoke this skill when any of the following is true:

- A new skill is created.
- An existing skill is modified.
- Periodic quality audit of skills is requested.
- A skill fails discovery, invocation, or behavior expectations.
- Two skills appear to overlap or contradict each other.

## Inputs

Required inputs:

- Target skill name or explicit list of target skills.
- Workspace skill root path.
- Review date.

Optional inputs:

- Previous review artifact path if already known.
- Change request or incident context.

## Required Outputs

- A per-skill review report in workspace docs using the template at [skill-review-report-template.md](references/skill-review-report-template.md).
- A conflict report when needed using [skill-conflict-report-template.md](references/skill-conflict-report-template.md).
- Updated per-skill history in [references/history](references/history/README.md).
- Review result summaries MUST be returned in Markdown grid format (tables), not prose lists.
- Aggregate multi-skill results MUST include at least one consolidated grid with per-skill outcomes.
- Per-skill review files MUST be stored under .docs/changes/skill-reviews/<skill-name>/.

## Assets

- Script assets are available in [references/scripts](references/scripts/README.md).
- Use generate-baseline-skill-reviews.ps1 to run full baseline audits and history updates.
- Use get-skill-metadata-audit.ps1 for quick frontmatter/trigger/reference checks.

## Workflow

1. Resolve target skills and collect current SKILL.md files and related assets.
2. Load the per-skill history file from references/history before analysis.
3. Build a recommendation deny-list from history entries marked Rejected, Removed, or Illegitimate.
4. Evaluate all SKR-M* and SKR-S* standards with evidence.
5. Produce pass or fail for MUST standards and advisory outcome for SHOULD standards.
6. If conflict is detected:
   - Document conflict using workspace documentation standards.
   - Recommend one or more concrete resolution options.
   - Work with the human user to choose and confirm the resolution.
7. Update the skill history file with findings, decisions, and recommendation statuses.

## Output Format Rules

- MUST return review outcomes in Markdown grid format.
- MUST present aggregate results using a single consolidated grid with these columns:
   - Skill
   - Outcome
   - MUST Failures
   - SHOULD Advisories
   - Conflict Status
   - Report
- SHOULD include a small aggregate metrics grid for totals (Pass, Pass With Advisories, Fail, Blocked).
- SHOULD keep narrative concise and place it after the grids.

## Storage Rules

- Store each skill review under .docs/changes/skill-reviews/<skill-name>/.
- Use file naming format YYYYMMDD-review.md for per-skill review artifacts.
- Keep aggregate cross-skill summaries in .docs/changes/skill-reviews/.

## Conflict Workflow (Mandatory When Conflict Exists)

When a conflict exists, this skill MUST:

1. Document the conflict in a workspace document at .docs/changes/skill-conflicts/ using the conflict template.
2. Provide recommendations to resolve the conflict.
3. Collaborate with the human user to select and confirm the final resolution.

Do not auto-resolve without explicit user direction.

## History Management Rules

- Keep one history file per reviewed skill in references/history.
- Use filename format: <skill-name>-history.md.
- Append new review entries; never rewrite prior decisions.
- Track each recommendation with status: Proposed, Accepted, Rejected, Removed, Implemented.
- Before publishing recommendations, remove any item that matches prior Rejected or Removed entries unless the user explicitly re-opens it.

## Decision Gates

- Gate 1 (MUST gate): Any SKR-M failure sets overall outcome to Fail.
- Gate 2 (SHOULD gate): SHOULD failures set outcome to Pass With Advisories if all MUST checks pass.
- Gate 3 (Conflict gate): Any unresolved conflict sets outcome to Blocked until user-confirmed resolution.

## Review Cadence

Recommended cadence:

- On skill creation.
- On skill update.
- Monthly for active skill libraries.
- Immediately after discovery or invocation failures.

## Done Criteria

A review is complete only when:

- All MUST and SHOULD checks were executed with evidence.
- Required report artifacts were written.
- History was updated for every reviewed skill.
- Conflicts were either resolved with user confirmation or explicitly marked Blocked.
