# Acceptance Criteria

## Repository

- [x] Git repository initialized.
- [x] Branch is `main`.
- [x] Apache-2.0 license present.
- [x] README explains value proposition, install, usage, profiles, and examples.
- [x] CONTRIBUTING, SECURITY, CODE_OF_CONDUCT, SUPPORT, and CHANGELOG present or intentionally deferred with TODOs.

## Templates

- [x] `Aegis.Modulith.Templates` builds as a NuGet template package.
- [x] `dotnet new install` works from local package.
- [x] `aegis-modulith` generates a solution.
- [x] `aegis-module` generates a module.
- [x] `aegis-slice` generates command/query slices.
- [x] `aegis-event` generates domain/integration events.
- [x] `aegis-worker` generates worker skeleton.

## Profile validation

- [x] Core profile builds and tests.
- [x] Pro profile builds and tests.
- [x] Advanced profile builds and tests.
- [x] Core mediator builds and tests.
- [x] MediatR option builds and tests.
- [x] TaskHub sample builds and tests.

## Architecture

- [x] Modular monolith structure present.
- [x] Vertical slices inside modules.
- [x] CQRS-lite contracts present.
- [x] PostgreSQL default present.
- [x] Schema-per-module design documented.
- [x] Architecture tests verify module boundaries.
- [x] Event sourcing is not default and is documented as advanced/optional.
- [x] Module manifests are generated for business modules.

## Spec-driven development

- [x] `specs/README.md` present.
- [x] `specs/_template` contains spec, plan, tasks, acceptance, risks, and open-questions templates.
- [x] At least one concrete spec exists.
- [x] `npm run check:specs` validates specs.
- [x] Spec workflows exist for create, review, and implement.

## AI development system

- [x] AGENTS.md present.
- [x] OpenQuestions.md present with required sections.
- [x] CLAUDE.md points to AGENTS.md.
- [x] Copilot instructions point to AGENTS.md.
- [x] `.agents/skills` contains SKILL.md files with metadata.
- [x] `.ai/workflows` contains workflows.
- [x] `.ai/policies` contains risk, dependency, security, and approval policies.
- [x] docs/open-questions-policy.md explains blocker versus inferred decisions.
- [x] Node `.mjs` guardrail runner present.
- [x] No duplicated `.sh`/`.ps1` guardrail logic.
- [x] Module manifest docs and template exist.

## CI and guardrails

- [x] `npm run check` passes.
- [x] `npm run check:ai` passes.
- [x] `npm run check:docs` passes.
- [x] `npm run check:security` passes.
- [x] `npm run check:specs` passes.
- [x] `npm run template:smoke` passes or clearly documents unavailable SDK/package blocker.
- [x] GitHub Actions workflows present.

## Public polish

- [x] `docs/competitive-analysis.md` present.
- [x] `docs/getting-started/which-profile.md` present.
- [x] `docs/public-polish-checklist.md` present.
- [x] README has golden path examples.
- [x] README has profile matrix.
- [x] README has template smoke matrix.

## Final report

- [ ] Codex reports commit list.
- [ ] Codex reports validation commands and outcomes.
- [ ] Codex reports known limitations.
- [ ] Codex reports open blockers and inferred assumptions from OpenQuestions.md.
- [ ] Codex reports specs used or updated.
- [ ] Codex reports next recommended release steps.
