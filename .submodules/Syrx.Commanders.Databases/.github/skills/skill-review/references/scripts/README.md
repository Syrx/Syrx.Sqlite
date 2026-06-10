# Skill Review Scripts

These scripts are reusable assets for the skill-review workflow.

## Scripts

- generate-baseline-skill-reviews.ps1
  - Purpose: Runs a baseline review across all workspace skills, generates per-skill reports in .docs/changes/skill-reviews/<skill-name>/YYYYMMDD-review.md, and updates history files plus history index.
  - Usage:
    - pwsh ./.github/skills/skill-review/references/scripts/generate-baseline-skill-reviews.ps1
    - pwsh ./.github/skills/skill-review/references/scripts/generate-baseline-skill-reviews.ps1 -RootPath c:/Projects/agentic_templates -ReviewDate 2026-03-28

- get-skill-metadata-audit.ps1
  - Purpose: Produces a quick metadata matrix for frontmatter, trigger discoverability, and references availability.
  - Usage:
    - pwsh ./.github/skills/skill-review/references/scripts/get-skill-metadata-audit.ps1
    - pwsh ./.github/skills/skill-review/references/scripts/get-skill-metadata-audit.ps1 -SkillsRoot c:/Projects/agentic_templates/.github/skills

- build-skill-review-grid.ps1
  - Purpose: Rebuilds the aggregate full-skill review grid from per-skill review folders.
  - Usage:
    - pwsh ./.github/skills/skill-review/references/scripts/build-skill-review-grid.ps1
    - pwsh ./.github/skills/skill-review/references/scripts/build-skill-review-grid.ps1 -RootPath c:/Projects/agentic_templates -ReviewDate 2026-03-28

## Notes

- These scripts are intended as review support assets (SKR-S1) and should be versioned with the skill.
- The script in .docs/changes/skill-reviews/_generate_skill_reviews.ps1 is the original run artifact from the first baseline audit.
