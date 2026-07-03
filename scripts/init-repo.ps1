$ErrorActionPreference = "Stop"

New-Item -ItemType Directory -Force -Path `
    "ai/context", "ai/plans", "ai/tasks", "ai/reports", `
    "docs", "references", "traces/samples", "traces/sanitized", `
    "scripts", "src", "tests", ".codex" | Out-Null

Write-Host "Repository directory structure is initialized."
