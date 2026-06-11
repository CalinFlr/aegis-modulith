# P1A Post-Fix Verification

Date: 2026-06-08

## Summary

P1A remains confirmed closed.

This verification reviewed the generated output semantics for:

- `--ai none|agents|enterprise`;
- `--guardrails off|standard|strict`;
- `--hooks none|lefthook`;
- `--skills none|core|enterprise`;
- `--docs standard|full`;
- `--license apache2|mit`.

No implementation fixes were made during this run.

Primary evidence came from the second immediate smoke run:

`artifacts/template-smoke/runs/mq5mbjlx-54188100`

The prior immediate run also passed:

`artifacts/template-smoke/runs/mq5m3wb5-b05743ff`

## P1A Pass/Fail Table

| Area | Result | Evidence |
| --- | --- | --- |
| AI option semantics | Pass | `ai-none`, `ai-agents`, `core-core`, and `strict-enterprise` generated the expected AI file shapes. |
| Guardrails option semantics | Pass | `guardrails-off-lefthook` omitted active Node guardrails; standard and strict outputs included and executed generated guardrails. |
| Hooks option semantics | Pass | `hooks=none` omitted `lefthook.yml`; `hooks=lefthook` generated a valid hook file, including dotnet-only hooks when guardrails are off. |
| Skills option semantics | Pass | `skills-none-docs-standard`, `skills-core-license-mit`, and enterprise defaults generated the expected skill sets. |
| Docs option semantics | Pass | `docs=standard` generated only standard docs; `docs=full` generated expanded docs without contradicting selected AI mode. |
| License option semantics | Pass | Apache-2.0 and MIT outputs generated matching `LICENSE`, README, package metadata, and build metadata. |
| Smoke quality | Pass | Smoke includes semantic positive and negative assertions, generated guardrail execution, isolated run directories, and no hardcoded local absolute paths. |

## Exact Generated-Output Evidence

All paths below are relative to `artifacts/template-smoke/runs/mq5mbjlx-54188100/g`.

### AI Options

`ai-none` recorded `ai=none`, `guardrails=standard`, `hooks=none`, `skills=enterprise`, `docs=full`, and `license=Apache-2.0` in `Directory.Build.props`.

The following AI-specific assets were absent from `ai-none`:

- `AGENTS.md`;
- `CLAUDE.md`;
- `OpenQuestions.md`;
- `.github/copilot-instructions.md`;
- `.ai`;
- `.agents`;
- `docs/ai-development`;
- `specs`.

`ai-agents` included:

- `AGENTS.md`;
- `CLAUDE.md`;
- `OpenQuestions.md`;
- `.github/copilot-instructions.md`;
- `docs/ai-development/agent-operating-model.md`;
- `docs/ai-development/ai-pr-protocol.md`.

`ai-agents` omitted enterprise-only AI assets:

- `.ai`;
- `.agents`;
- `specs`;
- `docs/ai-development/guardrails.md`;
- `docs/ai-development/skills.md`;
- `docs/ai-development/workflows.md`;
- `docs/ai-development/spec-driven-development.md`.

`core-core` and `strict-enterprise` represented `ai=enterprise` outputs. They included:

- `AGENTS.md`;
- `CLAUDE.md`;
- `OpenQuestions.md`;
- `.github/copilot-instructions.md`;
- `.ai/policies`;
- `.ai/workflows`;
- `.ai/guardrails`;
- `.ai/evals`;
- `.agents/skills`;
- `docs/ai-development`;
- `specs/README.md`;
- `specs/_template/spec.md`.

`strict-enterprise/.ai/policies` contained:

- `dependency-policy.md`;
- `forbidden-actions.yaml`;
- `risk-levels.md`;
- `security-policy.md`;
- `strict-mode.md`.

`strict-enterprise/.ai/workflows` contained:

- `create-module.md`;
- `create-spec.md`;
- `create-vertical-slice.md`;
- `implement-spec.md`;
- `pre-pr-review.md`;
- `review-spec.md`;
- `security-fix.md`.

### Guardrails Options

`guardrails-off-lefthook` recorded `guardrails=off` and omitted:

- `tools/guardrails/check.mjs`;
- `package.json`.

Its CI file only ran:

- `dotnet build -c Release --no-restore`;
- `dotnet test -c Release --no-build`.

It did not contain `npm run check`.

`core-core` recorded `guardrails=standard` and included:

- `tools/guardrails/check.mjs`;
- `package.json`;
- CI wiring for `npm run check`.

`core-core/package.json` included:

- `"check": "node tools/guardrails/check.mjs all"`;
- `"check:ai": "node tools/guardrails/check.mjs ai"`;
- `"check:docs": "node tools/guardrails/check.mjs docs"`;
- `"check:security": "node tools/guardrails/check.mjs security"`;
- `"check:specs": "node tools/guardrails/check.mjs specs"`;
- `"template:smoke": "node tools/guardrails/check.mjs template-smoke"`.

`core-core` did not include strict-only files:

- `.ai/policies/strict-mode.md`;
- `.ai/guardrails/strict-rules.md`.

`strict-enterprise` recorded `guardrails=strict` and included all standard guardrail assets plus:

- `.ai/policies/strict-mode.md`;
- `.ai/guardrails/strict-rules.md`;
- `.ai/policies/forbidden-actions.yaml`.

The generated strict policy and strict rules both describe stricter validation for:

- forbidden file patterns;
- sensitive files and key material;
- AI instruction consistency;
- skills, workflows, and specs shape.

The generated strict runner contained `checkStrict` and included checks for strict policy/rule files, forbidden action policy, sensitive-file rule documentation, AI instruction consistency, skills, workflows, and specs.

### Hooks Options

`core-core` recorded `hooks=none` and omitted `lefthook.yml`.

`strict-enterprise` recorded `hooks=lefthook` and generated:

```yaml
pre-commit:
  commands:
    guardrails:
      run: npm run check
```

`guardrails-off-lefthook` recorded `hooks=lefthook` with `guardrails=off` and generated:

```yaml
pre-commit:
  commands:
    dotnet-build:
      run: dotnet build -c Release
    dotnet-test:
      run: dotnet test -c Release --no-build
```

That file did not call `npm run check`.

### Skills Options

`skills-none-docs-standard` recorded `skills=none` and generated no `.agents/skills` directory.

`skills-core-license-mit` recorded `skills=core` and generated exactly six skills:

- `docs-writer`;
- `dotnet-architecture-review`;
- `dotnet-module`;
- `dotnet-vertical-slice`;
- `module-manifest`;
- `spec-driven-feature`.

It omitted enterprise-only skills:

- `competitive-review`;
- `efcore-migration-review`;
- `guardrail-runner`;
- `module-manifest-review`;
- `openapi-contract-review`;
- `security-review`.

`core-core` and `strict-enterprise` recorded `skills=enterprise` and generated twelve skills:

- `competitive-review`;
- `docs-writer`;
- `dotnet-architecture-review`;
- `dotnet-module`;
- `dotnet-vertical-slice`;
- `efcore-migration-review`;
- `guardrail-runner`;
- `module-manifest`;
- `module-manifest-review`;
- `openapi-contract-review`;
- `security-review`;
- `spec-driven-feature`.

`OpenQuestions.md` already contains inferred decision `Q-20260608-001`, defining the generated `--skills core` subset as:

- `docs-writer`;
- `dotnet-architecture-review`;
- `dotnet-module`;
- `dotnet-vertical-slice`;
- `module-manifest`;
- `spec-driven-feature`.

### Docs Options

`skills-none-docs-standard` recorded `docs=standard` and included only standard docs:

- `docs/getting-started.md`;
- `docs/architecture.md`;
- `docs/development.md`;
- `docs/module-manifest.md`.

It omitted expanded docs:

- `docs/ai-development`;
- `docs/security.md`;
- `docs/operations.md`;
- `docs/adr`.

`core-core`, `strict-enterprise`, and `skills-core-license-mit` recorded `docs=full` and included:

- standard docs;
- `docs/security.md`;
- `docs/operations.md`;
- `docs/adr`;
- `docs/ai-development/agent-operating-model.md`;
- `docs/ai-development/ai-pr-protocol.md`;
- `docs/ai-development/guardrails.md`;
- `docs/ai-development/skills.md`;
- `docs/ai-development/spec-driven-development.md`;
- `docs/ai-development/workflows.md`.

`ai-none` with `docs=full` still omitted `docs/ai-development`, so full docs did not contradict `--ai none`.

`ai-agents` with `docs=full` included only the basic AI docs and omitted enterprise AI docs, so full docs did not contradict `--ai agents`.

### License Options

`core-core` recorded `license=Apache-2.0`.

Evidence:

- `core-core/LICENSE` starts with `Apache License`, `Version 2.0, January 2004`, and `https://www.apache.org/licenses/`;
- `core-core/README.md` reports `License: Apache-2.0`;
- `core-core/package.json` reports `"license": "Apache-2.0"`;
- `core-core/Directory.Build.props` reports `<AegisLicense>Apache-2.0</AegisLicense>`.

`skills-core-license-mit` recorded `license=MIT`.

Evidence:

- `skills-core-license-mit/LICENSE` starts with `MIT License`;
- `skills-core-license-mit/README.md` reports `License: MIT`;
- `skills-core-license-mit/package.json` reports `"license": "MIT"`;
- `skills-core-license-mit/Directory.Build.props` reports `<AegisLicense>MIT</AegisLicense>`.

## Smoke Assertions Reviewed

`tools/guardrails/check.mjs` contains semantic assertion functions for the P1A surface:

- `assertAiSemantics`;
- `assertGuardrailSemantics`;
- `assertHookSemantics`;
- `assertSkillSemantics`;
- `assertDocsSemantics`;
- `assertLicenseSemantics`.

Reviewed assertion coverage includes:

- negative assertions for `ai=none`, `guardrails=off`, `hooks=none`, and `skills=none`;
- positive assertions for `ai=enterprise`, `guardrails=strict`, `hooks=lefthook`, `skills=enterprise`, `docs=full`, and both license modes;
- generated guardrail execution for every generated output where `guardrails != off`;
- standard-vs-strict checks that strict artifacts are absent outside strict enterprise output.

The smoke matrix includes targeted P1A variants:

- `ai-none`;
- `ai-agents`;
- `core-core`;
- `strict-enterprise`;
- `guardrails-off-lefthook`;
- `skills-none-docs-standard`;
- `skills-core-license-mit`.

Generated guardrails are executed from each standard/strict output by running:

`process.execPath tools/guardrails/check.mjs all`

The smoke runner uses an isolated run directory under `artifacts/template-smoke/runs/<run-id>`, generated with a timestamp and `randomUUID`, and isolates `DOTNET_CLI_HOME` and `NUGET_PACKAGES` under that run directory.

Search review found no hardcoded local absolute paths such as `C:\`, `C:/`, `/home/`, `/Users/`, `organization-specific-marker`, or a user profile path in `tools/guardrails/check.mjs`. The runner invokes commands with `shell: false` and does not depend on PowerShell, `cmd.exe`, or Windows-only shell syntax.

## Checks Run

| Command | Result | Evidence |
| --- | --- | --- |
| `npm run check` | Pass | All repository guardrails passed. |
| `npm run template:smoke` | Pass | Run directory: `artifacts/template-smoke/runs/mq5m3wb5-b05743ff`. |
| `npm run template:smoke` again immediately | Pass | Run directory: `artifacts/template-smoke/runs/mq5mbjlx-54188100`. |

## P1A Closure

P1A is confirmed closed.

The generated application output now honestly matches the documented AI, guardrails, hooks, skills, docs, and license CLI options within the P1A scope.

## Remaining P1B/P1C/P1D/P2 Gaps

This verification did not start or close work outside P1A.

Remaining gaps:

- P1B item template polish;
- P1C architecture-test expansion;
- P1D pro/advanced feature-depth work;
- P2 public polish such as public screenshots, badges, docs site, release presentation, and broader public-facing polish.

Event sourcing was not added.

No UI work was added.

The project was not renamed.

## Validation Limitations

- Verification used repository guardrails, the generated smoke matrix, static review of template/runner assertions, and targeted inspection of generated outputs.
- The smoke commands were executed on Windows. The runner itself avoids hardcoded local absolute paths and Windows shell assumptions, but this run did not execute the smoke matrix on Linux or macOS.
- This run did not manually inspect every line of every generated file.
- This run intentionally did not implement fixes.

## OpenQuestions.md Updates

No `OpenQuestions.md` changes were made during this verification.

No new true blockers or inferred decisions were identified.
