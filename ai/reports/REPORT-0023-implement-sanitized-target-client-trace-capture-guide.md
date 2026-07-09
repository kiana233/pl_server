# REPORT-0023 Implement Sanitized Target Client Trace Capture Guide

## Task ID

TASK-0023-implement-sanitized-target-client-trace-capture-guide

## Summary

Created documentation, safety rules, evidence promotion rules, and fake JSONL templates for future target-client trace capture and review. This task does not implement a capture tool and does not add real target-client trace.

No client resources, reference server source, raw captures, real traces, account names, passwords, tokens, IP addresses, device identifiers, databases, binaries, real account authentication, login response, character list response, gameplay behavior, or GUI behavior were added.

## Changed Files

- `.gitignore`
- `docs/trace-capture-guide.md`
- `docs/trace-sanitization-policy.md`
- `docs/trace-evidence-promotion-policy.md`
- `ai/context/current-state.md`
- `ai/context/latest-status.md`
- `ai/context/target-client-trace-requirements.md`
- `ai/tasks/TASK-0023-implement-sanitized-target-client-trace-capture-guide.md`
- `ai/reports/REPORT-0023-implement-sanitized-target-client-trace-capture-guide.md`
- `traces/templates/README.md`
- `traces/templates/sanitized-trace-event.example.jsonl`

## Added Documents

- `docs/trace-capture-guide.md` defines safe capture goals, pre-capture checks, forbidden sensitive data, recommended scenarios, and minimum event fields.
- `docs/trace-sanitization-policy.md` defines redaction rules and commit safety checks.
- `docs/trace-evidence-promotion-policy.md` defines when evidence may move toward `trace:client confirmed`.
- `ai/context/target-client-trace-requirements.md` lists P0 and P1 trace needs for future protocol work.
- `traces/templates/README.md` documents that templates are schema examples only.

## Sanitization Rules

- Delete or replace account names and character names.
- Delete passwords, tokens, cookies, session keys, and authentication secrets.
- Delete real IP addresses, MAC addresses, machine names, paths, and device identifiers.
- Delete chat content, private messages, personal notes, and personal data.
- Redact sensitive values in `rawHex` or omit the event if field-level redaction is not possible.
- Do not commit uncertain data.

## Evidence Promotion Rules

- `reference:muayad` cannot automatically become confirmed.
- `reference:wlophoenix` cannot automatically become confirmed.
- Synthetic tests cannot become confirmed.
- Sanitized target-client trace can become candidate evidence.
- `trace:client confirmed` requires target-client source, sanitization, replay, PacketCodec/Replay/Session agreement, human review, and a task report.

## Trace Template Notes

- `traces/templates/sanitized-trace-event.example.jsonl` is one JSON object on one line.
- The template uses fake sample data only.
- The template uses `sourceLabel: sample:synthetic`.
- The template uses `evidenceStatus: sample-only`.
- The template says it is not real client trace and not confirmed.
- The template contains no account, password, token, real IP, or device data.

## Gitignore Updates

Added ignore rules for:

- `traces/private/`
- `traces/unreviewed/`
- `traces/**/*.real.jsonl`
- `traces/**/*.private.jsonl`
- `*.har`

Existing ignore coverage already included `traces/raw/`, `*.pcap`, and `*.pcapng`.

## Commands Run

- `git status --short`
- `git checkout main`
- `git pull origin main`
- `git checkout -B task/0023-implement-sanitized-target-client-trace-capture-guide`
- `dotnet --info`
- `dotnet build .\src\PlServer.sln`
- `dotnet test .\src\PlServer.sln`
- `git status --short`
- `git diff --stat`
- line-count check for modified Markdown, JSONL, and `.gitignore` files
- `git add docs traces/templates ai/context/current-state.md ai/context/latest-status.md ai/context/target-client-trace-requirements.md ai/tasks/TASK-0023-implement-sanitized-target-client-trace-capture-guide.md ai/reports/REPORT-0023-implement-sanitized-target-client-trace-capture-guide.md .gitignore`
- `git commit -m "TASK-0023 implement sanitized target client trace capture guide"`
- `git push -u origin task/0023-implement-sanitized-target-client-trace-capture-guide`

## Test Results

Passed:

- `dotnet build .\src\PlServer.sln`: succeeded with 0 warnings and 0 errors.
- `dotnet test .\src\PlServer.sln`: succeeded. Application tests passed 23 tests, Diagnostics tests passed 19 tests, and Network tests passed 66 tests.

## Line Count Check

- `.gitignore`: 53 lines
- `docs/trace-capture-guide.md`: 60 lines
- `docs/trace-sanitization-policy.md`: 41 lines
- `docs/trace-evidence-promotion-policy.md`: 44 lines
- `ai/context/current-state.md`: 90 lines
- `ai/context/latest-status.md`: 40 lines
- `ai/context/target-client-trace-requirements.md`: 63 lines
- `ai/tasks/TASK-0023-implement-sanitized-target-client-trace-capture-guide.md`: 41 lines
- `ai/reports/REPORT-0023-implement-sanitized-target-client-trace-capture-guide.md`: 135 lines after final report update
- `traces/templates/README.md`: 16 lines
- `traces/templates/sanitized-trace-event.example.jsonl`: 1 line, as required for JSONL one-object-per-line format

## Risks

- No real target-client trace exists yet, so protocol facts remain pending.
- Sanitization still requires human review when real traces become available.
- Future trace capture tooling must be designed separately and reviewed before use.

## Blockers

- Real target-client packet traces are not yet available.
- Human trace review workflow is documented but not yet exercised on real data.

## Next Recommended Task

`TASK-0024-implement-account-repository-skeleton`

## Branch

`task/0023-implement-sanitized-target-client-trace-capture-guide`

## Commit Hash

Final task commit hash is printed in the terminal completion output.

## Push Result

Final push result is printed in the terminal completion output.
