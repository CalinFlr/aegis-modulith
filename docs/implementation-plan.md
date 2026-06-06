# Implementation Plan

## Phase 0: Repository initialization

- Initialize git if needed.
- Set default branch to `main`.
- Add Apache-2.0 license.
- Add root README.
- Add AGENTS.md and AI docs.
- Add package metadata.
- Commit: `chore: initialize aegis modulith repository`

## Phase 1: Documentation backbone

- Add project brief.
- Add architecture docs.
- Add CLI spec.
- Add ADRs.
- Add AI development docs.
- Add spec-driven development docs.
- Add competitive analysis and public polish checklist.
- Add module manifest docs.
- Add contribution/security docs.
- Commit: `docs: add architecture and ai development specification`

## Phase 1a: Spec-driven development layer

- Add `specs/README.md`.
- Add `specs/_template`.
- Add initial concrete spec for the template core.
- Add create/review/implement spec workflows.
- Add spec-driven feature skill.
- Add guardrail checks for specs.
- Commit: `docs: add spec-driven development layer`

## Phase 2: Guardrail runner

- Implement Node `.mjs` guardrail runner.
- Add `package.json` scripts.
- Validate AGENTS.md/tool-specific pointer files.
- Validate skills and workflows.
- Validate docs presence.
- Validate specs presence and required sections.
- Validate module manifest template.
- Add security/sensitive file checks.
- Commit: `build: add node guardrail runner`

## Phase 3: Template package skeleton

- Create `Aegis.Modulith.Templates` package structure.
- Add main template `aegis-modulith`.
- Add item templates `aegis-module`, `aegis-slice`, `aegis-event`, `aegis-worker`.
- Add `.template.config/template.json` for each.
- Add `module.json` manifest template for generated business modules.
- Commit: `feat: add dotnet template package skeleton`

## Phase 4: Core profile

- Implement generated solution skeleton for core.
- Add API project.
- Add BuildingBlocks project.
- Add module structure.
- Add module manifests.
- Add core dispatcher.
- Add OpenAPI, ProblemDetails, validation, result pattern.
- Add PostgreSQL/EF Core structure.
- Add architecture tests.
- Commit: `feat: implement core profile`

## Phase 5: Pro profile

- Add Aspire AppHost and ServiceDefaults.
- Add outbox skeleton.
- Add audit skeleton.
- Add idempotency support.
- Add Testcontainers integration tests.
- Add Dockerfile.
- Add CI.
- Commit: `feat: implement pro profile`

## Phase 6: MediatR option

- Add conditional MediatR implementation.
- Keep feature/slice code as similar as possible between mediator modes.
- Validate smoke matrix.
- Commit: `feat: add optional mediatr support`

## Phase 7: Advanced profile

- Add advanced scaffolding.
- Add permission/auth skeleton.
- Add tenancy skeleton.
- Add deployment skeleton.
- Add strict guardrails.
- Add optional Lefthook config.
- Commit: `feat: add advanced enterprise profile`

## Phase 8: TaskHub sample

- Add sample modules: Projects, Tasks, Notifications, Audit.
- Add `module.json` for every sample module.
- Add representative command and query slices.
- Demonstrate domain event, integration event, outbox-ready flow.
- Add tests and docs.
- Commit: `feat: add taskhub reference sample`

## Phase 9: CI and template smoke tests

- Add GitHub Actions.
- Add template generation smoke tests.
- Add docs, specs, and security workflows.
- Commit: `ci: validate templates and guardrails`

## Phase 10: Polish

- Update README.
- Add badges placeholders.
- Add golden path examples.
- Add public polish checklist status.
- Add roadmap.
- Add release checklist.
- Run all validation.
- Commit: `docs: polish public repository experience`
