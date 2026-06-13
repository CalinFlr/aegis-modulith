# Tasks

## Repository foundation

- [x] Initialize git and set branch to `main`.
- [x] Add Apache-2.0 license.
- [x] Add README, CONTRIBUTING, SECURITY, ROADMAP, CHANGELOG, SUPPORT, and CODE_OF_CONDUCT or intentional placeholders.
- [x] Add AGENTS.md, CLAUDE.md, Copilot pointer file, and OpenQuestions.md.

## Spec and AI system

- [x] Keep `specs/` templates and initial core spec.
- [x] Add skills, workflows, policies, guardrails, and evals.
- [x] Ensure `npm run check` validates specs, skills, workflows, manifests, docs, and security.

## Template package

- [x] Create `Aegis.Modulith.Templates`.
- [x] Implement `aegis-modulith`.
- [x] Implement `aegis-module`.
- [x] Implement `aegis-slice`.
- [x] Implement `aegis-event`.
- [x] Implement `aegis-worker`.

## Generated solution

- [x] Generate API-only .NET solution.
- [x] Add CQRS-lite contracts and core dispatcher.
- [x] Add PostgreSQL schema-per-module persistence.
- [x] Add OpenAPI, ProblemDetails, validation, health checks, logging, and OpenTelemetry.
- [x] Add architecture tests and integration test skeleton.

## Profiles and options

- [x] Implement `core` profile.
- [x] Implement `pro` profile.
- [x] Implement `advanced` profile.
- [x] Implement `--mediator mediatr`.
- [x] Implement `--sample taskhub`.
- [x] Implement generated `--ai none|agents|enterprise` file-shape materialization.
- [x] Implement generated `--guardrails off|standard|strict` runner, package, CI, and strict artifact materialization.
- [x] Implement generated `--hooks none|lefthook` materialization, including dotnet-only hooks when guardrails are off.
- [x] Implement generated `--skills none|core|enterprise` skill-set materialization.
- [x] Implement generated `--docs standard|full` docs materialization.
- [x] Implement generated `--license apache2|mit` license materialization.
- [x] Implement P1D-2B inbox/idempotency scaffold for generated pro and advanced profiles.

## Validation and polish

- [x] Add template smoke matrix.
- [x] Add CI workflows.
- [x] Add docs for profile choice and module manifests.
- [x] Add competitive analysis and public positioning.
- [ ] Produce final report.
