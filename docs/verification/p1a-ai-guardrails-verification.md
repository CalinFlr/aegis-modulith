# P1A AI and Guardrails Verification

Date: 2026-06-08

## Summary

P1A is implemented for generated enterprise AI assets and guardrail option materialization.

The generated `aegis-modulith` output now has semantic file-shape behavior for:

- `--ai none|agents|enterprise`;
- `--guardrails off|standard|strict`;
- `--hooks none|lefthook`;
- `--skills none|core|enterprise`;
- `--docs standard|full`;
- `--license apache2|mit`.

## Implemented Behavior

| Option | Verified generated behavior |
| --- | --- |
| `--ai none` | Excludes `AGENTS.md`, `CLAUDE.md`, `.github/copilot-instructions.md`, `OpenQuestions.md`, `.ai`, `.agents`, AI docs, and `specs`. |
| `--ai agents` | Includes `AGENTS.md`, `CLAUDE.md`, Copilot pointer, `OpenQuestions.md`, and basic AI docs when `--docs full`; excludes enterprise `.ai`, `.agents`, and `specs`. |
| `--ai enterprise` | Includes agent files, `.ai/policies`, `.ai/workflows`, `.ai/guardrails`, `.ai/evals`, generated skills according to `--skills`, AI docs according to `--docs`, and `specs/README.md` plus `specs/_template`. |
| `--guardrails off` | Excludes `tools/guardrails/check.mjs`, generated `package.json`, and guardrail CI wiring. |
| `--guardrails standard` | Includes generated Node runner, package scripts, generated AI/docs/security/spec/skill/workflow checks, and CI wiring. |
| `--guardrails strict` | Includes standard guardrails plus strict policy/rule files and strict generated checks. |
| `--hooks none` | Excludes `lefthook.yml`. |
| `--hooks lefthook` | Uses `npm run check` when guardrails are enabled; uses dotnet build/test hooks when guardrails are off. |
| `--skills none` | Excludes generated `.agents/skills`. |
| `--skills core` | Includes the inferred core skill subset recorded in `OpenQuestions.md`. |
| `--skills enterprise` | Includes the full generated enterprise skill set. |
| `--docs standard` | Includes getting-started, architecture, development, and module-manifest docs only. |
| `--docs full` | Adds AI development docs when AI is enabled, plus security, operations, and ADR docs. |
| `--license apache2` | Generates Apache-2.0 `LICENSE`, README license text, and package metadata. |
| `--license mit` | Generates MIT `LICENSE`, README license text, and package metadata. |

## Smoke Assertions Added

`npm run template:smoke` now generates and validates additional variants for:

- `ai=none`;
- `ai=agents`;
- default `ai=enterprise`;
- `guardrails=off` with `hooks=lefthook`;
- `guardrails=standard`;
- `guardrails=strict`;
- `skills=none`;
- `skills=core`;
- default `skills=enterprise`;
- `docs=standard`;
- default `docs=full`;
- `license=mit`;
- default `license=apache2`.

For generated outputs with standard or strict guardrails, smoke also runs the generated Node guardrail runner directly.

## Validation

Validation completed before this report:

| Command | Result | Notes |
| --- | --- | --- |
| `dotnet pack templates/Aegis.Modulith.Templates/Aegis.Modulith.Templates.csproj -c Release -o artifacts/p1a-pack-check` | Pass | Template package created. |
| Targeted `dotnet new aegis-modulith` generation for ai-none, strict, MIT, agents, and guardrails-off+lefthook | Pass | File-shape spot checks matched expected behavior. |
| Generated `npm run check` for targeted ai-none, strict, MIT, and agents outputs | Pass | Generated guardrail runner passed. |
| `npm run template:smoke` | Pass | Run directory: `artifacts/template-smoke/runs/mq57k9l0-511e64a3`. |
| `npm run check` | Pass | Required final validation. |
| `npm run template:smoke` | Pass | Required final smoke run: `artifacts/template-smoke/runs/mq57zg92-5f58238b`. |
| `npm run template:smoke` again immediately | Pass | Required idempotency run: `artifacts/template-smoke/runs/mq589s64-c14d4c2d`. |

## OpenQuestions.md

`OpenQuestions.md` changed.

Added inferred decision `Q-20260608-001` for the generated `--skills core` subset.

## Remaining Gaps

P1A does not close P1B/P1C/P1D/P2.

Remaining known gaps include:

- P1B item template polish;
- broader pro/advanced feature depth beyond current buildable scaffolding;
- public polish such as screenshots, badges, docs site, and release presentation;
- deeper architecture tests for every documented architecture rule.

Event sourcing was not added.
