$ErrorActionPreference = "Stop"

if (Test-Path "src/PlServer.sln") {
    $solution = Get-Content "src/PlServer.sln" -Raw
    if ($solution -match "Project\(") {
        dotnet test "src/PlServer.sln"
    } else {
        Write-Host "Solution exists but has no projects yet; no tests to run."
    }
} else {
    Write-Host "No solution file exists yet; no tests to run."
}
