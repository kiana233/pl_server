$ErrorActionPreference = "Stop"

git status --short | Out-File "ai/context/git-status.txt" -Encoding utf8
$gitLog = git log --oneline -30 2>$null
if (-not $gitLog) {
    $gitLog = "No commits yet."
}
$gitLog | Out-File "ai/context/git-log.txt" -Encoding utf8
git diff --stat | Out-File "ai/context/git-diff-stat.txt" -Encoding utf8
git ls-files | Out-File "ai/context/file-list.txt" -Encoding utf8

$summary = @"
# Repository Status Summary

## Git Status

$(Get-Content "ai/context/git-status.txt" -Raw)

## Recent Commits

$(Get-Content "ai/context/git-log.txt" -Raw)

## Diff Stat

$(Get-Content "ai/context/git-diff-stat.txt" -Raw)

## File List

$(Get-Content "ai/context/file-list.txt" -Raw)
"@

$summary | Out-File "ai/context/repo-summary.md" -Encoding utf8
