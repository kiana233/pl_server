# Task Report Standard

Every task report under `ai/reports/` must include the following sections:

- Task ID
- Summary
- Changed Files
- Design Decisions
- Reference Used
- Commands Run
- Test Results
- Manual Test Results
- Risks
- Blockers
- Next Recommended Task
- Branch
- Commit Hash
- Push Result

## Guidance

- Reports should state whether the task changed code, docs, tests, or only metadata.
- Build/test failures must include the exact reason when known.
- Reports must not include secrets, private packet data, real account passwords, committed client resources, or copied reference server source.
- If a commit hash is not knowable until after the report is written, the report should state that the final hash is recorded in the terminal/final task output.
