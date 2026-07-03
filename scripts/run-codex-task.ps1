param(
    [string]$TaskFile
)

$ErrorActionPreference = "Stop"

if (-not $TaskFile) {
    throw "Usage: ./scripts/run-codex-task.ps1 -TaskFile ai/tasks/TASK-xxxx.md"
}

git pull

$agents = Get-Content "AGENTS.md" -Raw
$status = Get-Content "ai/context/latest-status.md" -Raw
$task = Get-Content $TaskFile -Raw

$prompt = @"
You are Codex working inside the pl_server repository.

Follow AGENTS.md strictly.

===== AGENTS.md =====
$agents

===== latest-status.md =====
$status

===== Active Task =====
$task

Implement only the active task.
Do not modify unrelated files.
Do not commit binaries or client resources.
Run relevant tests if possible.
Create or update the required report under ai/reports/.
Do not commit unless explicitly instructed.
"@

$prompt | codex exec --cd . --sandbox workspace-write -
