# Open Questions Protocol

`OpenQuestions.md` is the repository-level human-agent decision ledger.

The protocol is simple:

1. Read `OpenQuestions.md` before substantial work.
2. Continue when a safe default exists.
3. Record non-blocking assumptions with status `inferred`.
4. Record human preference questions with status `open`.
5. Record true blockers with status `blocker`.
6. Stop only when a high or critical risk blocker prevents safe implementation or validation.
7. Move answered questions to `answered` or `decided` after the human responds.
8. Promote durable architecture decisions into ADRs.
9. Mention unresolved blockers and inferred assumptions in the final report.

See `docs/open-questions-policy.md` for the full policy and `OpenQuestions.md` for the active queue.
