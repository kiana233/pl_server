$ErrorActionPreference = "Stop"

if (-not (Test-Path "ai/context/latest-status.md")) {
    throw "Missing ai/context/latest-status.md"
}

Write-Host "Context files are present. Update task-specific facts manually."
