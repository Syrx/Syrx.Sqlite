# Skill Review History Store

This folder stores review and recommendation history per skill.

## Rules

- Create one file per skill using: <skill-name>-history.md
- Initialize each file using ../history-entry-template.md
- Append entries in chronological order
- Never delete prior entries; supersede with new entries
- Mark recommendation lifecycle using statuses:
  - Proposed
  - Accepted
  - Rejected
  - Removed
  - Implemented

## Pre-Review Requirement

Before issuing new recommendations for a skill:

1. Load that skill history file into context.
2. Build a deny-list from Rejected, Removed, and Illegitimate recommendation records.
3. Filter out repeated recommendations unless the user explicitly re-opens them.
