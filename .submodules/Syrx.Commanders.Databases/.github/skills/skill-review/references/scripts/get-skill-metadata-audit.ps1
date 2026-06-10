param(
    [string]$SkillsRoot = 'c:/Projects/agentic_templates/.github/skills'
)

$ErrorActionPreference = 'Stop'

$skills = Get-ChildItem $SkillsRoot -Directory |
    Sort-Object Name |
    Where-Object { Test-Path (Join-Path $_.FullName 'SKILL.md') }

$rows = foreach ($s in $skills) {
    $skillFile = Join-Path $s.FullName 'SKILL.md'
    $content = Get-Content $skillFile -Raw

    $frontMatch = [regex]::Match($content, '(?s)^---\r?\n(.*?)\r?\n---')
    $front = if ($frontMatch.Success) { $frontMatch.Groups[1].Value } else { '' }

    $hasFrontmatter = $frontMatch.Success
    $hasName = $front -match '(?m)^name\s*:'
    $hasDescription = $front -match '(?m)^description\s*:'

    $descriptionStartsUseWhen =
        (($front -match '(?im)^description\s*:\s*.*Use when') -or
        (($front -match '(?im)^description\s*:\s*>\s*$') -and ($front -match '(?i)Use when')))

    [pscustomobject]@{
        Skill = $s.Name
        HasFrontmatter = $hasFrontmatter
        HasName = $hasName
        HasDescription = $hasDescription
        DescriptionStartsUseWhen = $descriptionStartsUseWhen
        HasTriggerSection = ($content -match '(?im)^##\s+(When to Use|Trigger Conditions|Typical Use Cases|Use Cases)')
        HasReferences = (Test-Path (Join-Path $s.FullName 'references'))
    }
}

$rows | Sort-Object Skill | Format-Table -AutoSize | Out-String
