param(
    [string]$RootPath = 'c:/Projects/agentic_templates',
    [string]$ReviewDate = '2026-03-28'
)

$ErrorActionPreference = 'Stop'

$reportsDir = Join-Path $RootPath '.docs/changes/skill-reviews'
$outFile = Join-Path $reportsDir ("{0}-full-skill-review-grid.md" -f ($ReviewDate -replace '-', ''))

$rows = Get-ChildItem $reportsDir -Recurse -File -Filter '*-review.md' |
    ForEach-Object {
        if ($_.FullName -eq $outFile) { return }

        $text = Get-Content $_.FullName -Raw
        $skill = ([regex]::Match($text, '- Target Skill:\s*(.+)').Groups[1].Value.Trim())
        if ([string]::IsNullOrWhiteSpace($skill)) { return }

        [pscustomobject]@{
            Skill = $skill
            Outcome = ([regex]::Match($text, '\| Overall Outcome \|\s*(.+?)\s*\|').Groups[1].Value.Trim())
            MustFailures = [int]([regex]::Match($text, '\| MUST Failures \|\s*(\d+)\s*\|').Groups[1].Value.Trim())
            ShouldAdvisories = [int]([regex]::Match($text, '\| SHOULD Advisories \|\s*(\d+)\s*\|').Groups[1].Value.Trim())
            ConflictStatus = ([regex]::Match($text, '\| Conflict Status \|\s*(.+?)\s*\|').Groups[1].Value.Trim())
            ReportPath = (Resolve-Path -Relative $_.FullName).Replace('\\', '/').TrimStart('.', '/')
        }
    } |
    Sort-Object Skill

$total = $rows.Count
$pass = ($rows | Where-Object Outcome -eq 'Pass').Count
$passWithAdvisories = ($rows | Where-Object Outcome -eq 'Pass With Advisories').Count
$fail = ($rows | Where-Object Outcome -eq 'Fail').Count
$blocked = ($rows | Where-Object Outcome -eq 'Blocked').Count
$mustTotal = ($rows | Measure-Object MustFailures -Sum).Sum
$advisoryTotal = ($rows | Measure-Object ShouldAdvisories -Sum).Sum

$lines = @()
$lines += '# Full Skill Review Grid'
$lines += ''
$lines += '## Aggregate Outcome Grid'
$lines += ''
$lines += '| Metric | Value |'
$lines += '|---|---|'
$lines += "| Review Date | $ReviewDate |"
$lines += "| Total Skills Reviewed | $total |"
$lines += "| Pass | $pass |"
$lines += "| Pass With Advisories | $passWithAdvisories |"
$lines += "| Fail | $fail |"
$lines += "| Blocked | $blocked |"
$lines += "| Total MUST Failures | $mustTotal |"
$lines += "| Total SHOULD Advisories | $advisoryTotal |"
$lines += ''
$lines += '## Per-Skill Results Grid'
$lines += ''
$lines += '| Skill | Outcome | MUST Failures | SHOULD Advisories | Conflict Status | Report |'
$lines += '|---|---|---:|---:|---|---|'

foreach ($row in $rows) {
    $lines += "| $($row.Skill) | $($row.Outcome) | $($row.MustFailures) | $($row.ShouldAdvisories) | $($row.ConflictStatus) | $($row.ReportPath) |"
}

Set-Content -Path $outFile -Value ($lines -join "`n") -NoNewline
Write-Output "Updated $outFile"
