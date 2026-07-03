# 00 Project Background

This repository is for rebuilding a stable Wonderland Online compatible server foundation.

The current problem is not one missing function. The service must be rebuilt around a disciplined workflow because:

- protocol behavior is incomplete
- old server code is incomplete
- client state flow is complex
- reference servers are also incomplete
- symptom-based AC handler patching creates new regressions
- there has been no strict task boundary
- replay tests are missing
- acceptance criteria are missing

The rebuild must be task-driven, protocol-fact-driven, and replay-test-driven.
