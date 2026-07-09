# Trace Evidence Promotion Policy

This policy defines how protocol evidence may be promoted.

## Non-Promotable Evidence

- `reference:muayad` cannot automatically become `confirmed`.
- `reference:wlophoenix` cannot automatically become `confirmed`.
- Synthetic client tests cannot become `confirmed`.
- Assumptions cannot become `confirmed` without target-client evidence.

## Candidate Evidence

Sanitized target-client trace can be used as candidate evidence when:

- The source is clearly the target client.
- The trace has been sanitized.
- The event shape can be replayed or manually checked against PacketCodec behavior.
- The trace is tied to a task report.

## Promotion To Trace Client Confirmed

Only mark a behavior as `trace:client confirmed` when all of the following are true:

1. The trace source is explicitly the target client.
2. The trace is sanitized.
3. The trace can be replayed through the replay framework.
4. PacketCodec, Replay, and Session tests agree with the trace behavior.
5. Human review approved the evidence promotion.
6. A task report records the trace, review result, supported protocol facts, and remaining risks.

If there is only one sanitized trace, prefer `trace:client pending-review` until repeatability improves.

If target-client trace conflicts with reference behavior, target-client trace has priority, but the difference must be documented in the report and in protocol notes.

## Forbidden Promotion

Do not promote evidence when:

- The trace is synthetic.
- The trace is unreviewed.
- Sanitization is incomplete.
- The trace contains private data.
- Replay or PacketCodec behavior contradicts the claimed protocol fact without a documented explanation.
