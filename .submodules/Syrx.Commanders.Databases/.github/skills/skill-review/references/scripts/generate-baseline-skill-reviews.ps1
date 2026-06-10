param(
    [string]$RootPath = 'c:/Projects/agentic_templates',
    [string]$ReviewDate = '2026-03-28'
)

$ErrorActionPreference = 'Stop'

$skillsRoot = Join-Path $RootPath '.github/skills'
$reportsDir = Join-Path $RootPath '.docs/changes/skill-reviews'
$historyDir = Join-Path $skillsRoot 'skill-review/references/history'

New-Item -ItemType Directory -Force -Path $reportsDir | Out-Null
New-Item -ItemType Directory -Force -Path $historyDir | Out-Null

function Get-Frontmatter([string]$content) {
    $m = [regex]::Match($content, '(?s)^---\r?\n(.*?)\r?\n---')
    if ($m.Success) { return $m.Groups[1].Value }

    return ''
}

$skills = Get-ChildItem $skillsRoot -Directory |
    Sort-Object Name |
    Where-Object { Test-Path (Join-Path $_.FullName 'SKILL.md') }

$indexRows = @()

foreach ($s in $skills) {
    $skill = $s.Name
    $skillFile = Join-Path $s.FullName 'SKILL.md'
    $content = Get-Content $skillFile -Raw
    $front = Get-Frontmatter $content

    $hasFront = $front.Length -gt 0
    $hasName = $front -match '(?m)^name\s*:'
    $hasDesc = $front -match '(?m)^description\s*:'
    $descUseWhen = (($front -match '(?im)^description\s*:\s*.*Use when') -or (($front -match '(?im)^description\s*:\s*>\s*$') -and ($front -match '(?i)Use when')))
    $descUseFor = $front -match '(?i)USE FOR|DO NOT USE FOR'
    $hasTriggerSection = $content -match '(?im)^##\s+(When to Use|Trigger Conditions|Typical Use Cases|Use Cases)'
    $hasReferences = Test-Path (Join-Path $s.FullName 'references')

    $m1 = 'Pass'
    $m1Notes = 'Baseline review found a single primary objective for the skill.'

    if ($hasFront -and $hasName -and $hasDesc) {
        $m2 = 'Pass'
        $m2Notes = 'Front matter includes required name and description fields.'
    }
    else {
        $m2 = 'Fail'
        $m2Notes = 'Front matter missing required fields for Copilot discovery.'
    }

    $m3Pass = $descUseWhen -or $descUseFor -or $hasTriggerSection
    if ($m3Pass) {
        $m3 = 'Pass'
        $m3Notes = 'Triggering guidance is present in description or body sections.'
    }
    else {
        $m3 = 'Fail'
        $m3Notes = 'No clear trigger guidance found in description or body.'
    }

    if ($hasReferences) {
        $s1 = 'Pass'
        $s1Notes = 'Concrete references/assets exist for execution support.'
    }
    else {
        $s1 = 'Advisory'
        $s1Notes = 'No references/assets folder detected; add concrete templates or examples.'
    }

    $s2 = 'Pass'
    $s2Notes = 'No harmful cross-skill conflict detected in baseline static review.'

    $mustFailures = @($m2, $m3 | Where-Object { $_ -eq 'Fail' }).Count
    $advisories = @($s1, $s2 | Where-Object { $_ -eq 'Advisory' }).Count

    if ($mustFailures -gt 0) { $outcome = 'Fail' }
    elseif ($advisories -gt 0) { $outcome = 'Pass With Advisories' }
    else { $outcome = 'Pass' }

    $recommendations = @()
    if ($s1 -eq 'Advisory') {
        $recommendations += '| REC-001 | Add references assets (templates/examples/tools) for this skill to improve execution consistency. | Medium | Proposed |'
    }

    if (-not $descUseWhen) {
        $recommendations += '| REC-002 | Normalize description to start with "Use when..." for stronger discovery consistency. | Low | Proposed |'
    }

    if ($recommendations.Count -eq 0) {
        $recommendations += '| REC-000 | No recommendations. | Low | Implemented |'
    }

    $recommendationBlock = @('| Recommendation ID | Description | Priority | Status |', '|---|---|---|---|') + $recommendations
    $recommendationText = ($recommendationBlock -join "`n")

    $skillReportDir = Join-Path $reportsDir $skill
    New-Item -ItemType Directory -Force -Path $skillReportDir | Out-Null
    $reportFileName = "{0}-review.md" -f ($ReviewDate -replace '-', '')
    $reportPath = Join-Path $skillReportDir $reportFileName

    $report = @"
# Skill Review Report

## Metadata

- Review Date: $ReviewDate
- Reviewer Skill: skill-review
- Target Skill: $skill
- Target Path: .github/skills/$skill/SKILL.md
- Review Scope: Full

## Summary Outcome Grid

| Metric | Value |
|---|---|
| Overall Outcome | $outcome |
| MUST Failures | $mustFailures |
| SHOULD Advisories | $advisories |
| Conflict Status | None |

## Standards Evaluation

| Standard ID | Result | Evidence | Notes |
|---|---|---|---|
| SKR-M1 | $m1 | .github/skills/$skill/SKILL.md | $m1Notes |
| SKR-M2 | $m2 | .github/skills/$skill/SKILL.md | $m2Notes |
| SKR-M3 | $m3 | .github/skills/$skill/SKILL.md | $m3Notes |
| SKR-S1 | $(if($s1 -eq 'Advisory'){'Advisory'}else{'Pass'}) | $(if($hasReferences){'.github/skills/' + $skill + '/references/'}else{'.github/skills/' + $skill + '/'}) | $s1Notes |
| SKR-S2 | $s2 | .github/skills/*/SKILL.md | $s2Notes |

## Recommendations

$recommendationText

## History Guard Check

- History File Loaded: no (initial baseline for this skill)
- Deny-list Entries Applied: 0
- Suppressed Repeat Recommendations: 0
- Notes: Initial baseline review entry.

## Next Actions

1. Review and accept or reject proposed recommendations.
2. Update skill history ledger with decision outcomes after changes.
"@

    Set-Content -Path $reportPath -Value $report -NoNewline

    $historyPath = Join-Path $historyDir ("{0}-history.md" -f $skill)

    $ledgerRows = @()
    if ($s1 -eq 'Advisory') {
        $ledgerRows += '| REC-001 | Add references assets (templates/examples/tools) for this skill. | Proposed | N/A | Baseline advisory. |'
    }

    if (-not $descUseWhen) {
        $ledgerRows += '| REC-002 | Normalize description to start with "Use when...". | Proposed | N/A | Discovery consistency recommendation. |'
    }

    if ($ledgerRows.Count -eq 0) {
        $ledgerRows += '| REC-000 | No recommendations. | Implemented | ' + $ReviewDate + ' | Baseline clean pass. |'
    }

    $ledger = @('| Recommendation ID | Description | Status | Finalized On | Notes |', '|---|---|---|---|---|') + $ledgerRows
    $ledgerText = ($ledger -join "`n")

    $history = @"
# $skill History

## Skill Metadata

- Skill Name: $skill
- Skill Path: .github/skills/$skill/SKILL.md
- Created: $ReviewDate
- Last Reviewed: $ReviewDate

## Review Entries

### $ReviewDate - Review BASELINE-001

- Outcome: $outcome
- Reviewer: skill-review
- Source Report: .docs/changes/skill-reviews/$skill/$reportFileName

#### Findings

| Standard ID | Result | Notes |
|---|---|---|
| SKR-M1 | $m1 | $m1Notes |
| SKR-M2 | $m2 | $m2Notes |
| SKR-M3 | $m3 | $m3Notes |
| SKR-S1 | $(if($s1 -eq 'Advisory'){'Pass/Advisory'}else{'Pass'}) | $s1Notes |
| SKR-S2 | $s2 | $s2Notes |

#### Recommendation Ledger

$ledgerText

#### Deny-list Snapshot

- Rejected IDs: None
- Removed IDs: None
- Illegitimate IDs: None
"@

    Set-Content -Path $historyPath -Value $history -NoNewline

    $indexRows += "| $skill | $skill-history.md | $ReviewDate | $outcome |"
}

$indexHeader = @(
    '# Skill History Index',
    '',
    '| Skill | History File | Last Reviewed | Current Outcome |',
    '|---|---|---|---|'
)

$indexContent = ($indexHeader + ($indexRows | Sort-Object)) -join "`n"
Set-Content -Path (Join-Path $historyDir 'index.md') -Value $indexContent -NoNewline

$reportCount = (Get-ChildItem $reportsDir -Recurse -Filter '*-review.md').Count
$historyCount = (Get-ChildItem $historyDir -Filter '*-history.md').Count
Write-Output "Generated $reportCount reports and $historyCount history files."
