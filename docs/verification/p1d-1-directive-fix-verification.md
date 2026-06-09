# P1D-1 Directive Fix Verification

Date: 2026-06-09

## Summary

P1D-1 is confirmed closed for the previously failing documentation and smoke coverage findings.

Generated Markdown and YAML template conditionals are now processed for the main `aegis-modulith` template, and template smoke fails if unresolved `#if`, `#else`, `#elseif`, `#elif`, or `#endif` directives remain in generated main-template or item-template outputs.

## Fixes Verified

| Area | Result | Evidence |
| --- | --- | --- |
| Generated Markdown conditional processing | Pass | `docs/testing.md`, `README.md`, and `AGENTS.md` no longer retain template conditional markers in generated outputs. |
| Generated YAML conditional processing | Pass | `lefthook.yml` no longer retains template conditional markers when generated. |
| Core testing docs truthfulness | Pass | Generated `core-core/docs/testing.md` describes the core architecture-test-only surface and no longer names a nonexistent `tests/Smoke.CoreCore.IntegrationTests` project. |
| Pro/advanced testing docs truthfulness | Pass | Generated `pro-core/docs/testing.md` names `tests/Smoke.ProCore.IntegrationTests` and includes Testcontainers, fake authentication, and HttpClient resilience guidance. |
| Smoke unresolved-directive coverage | Pass | `npm run template:smoke` now scans generated main-template and item-template outputs for unresolved conditional directives. |
| Default smoke without Docker | Pass | Generated pro/advanced Docker-backed tests remain skipped by default unless `AEGIS_RUN_TESTCONTAINERS=true` is set. |

## Smoke Run

Latest smoke run:

```text
artifacts/template-smoke/runs/mq6m372p-4b886464
```

Targeted generated-output check:

```powershell
rg -n "^\s*(//|<!--)?\s*#(if|else|elseif|elif|endif)\b" artifacts/template-smoke/runs/mq6m372p-4b886464/g
```

Result: no matches.

Representative generated docs:

- `g/core-core/docs/testing.md` contains the core-only testing section and no integration-test project reference.
- `g/pro-core/docs/testing.md` contains the generated integration-test project reference and pro/advanced P1D-1 guidance.
- `g/guardrails-off-lefthook/lefthook.yml` contains only dotnet build/test hook commands and no conditional markers.

## Checks Run

| Command | Result |
| --- | --- |
| `node --check tools/guardrails/check.mjs` | Pass |
| `npm run template:smoke` | Pass |
| `npm run check` | Pass |
| `npm run check:specs` | Pass |

## Validation Limitations

Docker is not installed or not on `PATH`, so live Testcontainers execution was not run. This does not affect default smoke validation because Docker-backed generated tests are intentionally opt-in under `AEGIS_RUN_TESTCONTAINERS=true`.

## OpenQuestions.md Updates

`OpenQuestions.md` was not changed.

No new blockers or inferred decisions were identified. Existing relevant inferred decisions remain:

- `Q-20260609-002`: generated Testcontainers tests stay opt-in.
- `Q-20260609-003`: fake authentication stays test-only until P1D-2.

## Closure

The documentation truthfulness and unresolved generated directive failures from `docs/verification/p1d-1-post-fix-verification.md` are resolved.

Recommended next step: start P1D-2.
