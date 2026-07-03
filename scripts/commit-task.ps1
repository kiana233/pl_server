param(
    [string]$Message
)

$ErrorActionPreference = "Stop"

if (-not $Message) {
    throw "Usage: ./scripts/commit-task.ps1 -Message 'type(scope): summary'"
}

git status --short
git add .
git commit -m $Message
