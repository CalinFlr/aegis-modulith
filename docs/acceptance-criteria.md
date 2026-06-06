# Acceptance Criteria

## Repository

- [ ] Git repository initialized.
- [ ] Branch is `main`.
- [ ] Apache-2.0 license present.
- [ ] README explains value proposition, install, usage, profiles, and examples.
- [ ] CONTRIBUTING, SECURITY, CODE_OF_CONDUCT, SUPPORT, and CHANGELOG present or intentionally deferred with TODOs.

## Templates

- [ ] `Aegis.Modulith.Templates` builds as a NuGet template package.
- [ ] `dotnet new install` works from local package.
- [ ] `aegis-modulith` generates a solution.
- [ ] `aegis-module` generates a module.
- [ ] `aegis-slice` generates command/query slices.
- [ ] `aegis-event` generates domain/integration events.
- [ ] `aegis-worker` generates worker skeleton.

## Profile validation

- [ ] Core profile builds and tests.
- [ ] Pro profile builds and tests.
- [ ] Advanced profile builds and tests.
- [ ] Core mediator builds and tests.
- [ ] MediatR option builds and tests.
- [ ] TaskHub sample builds and tests.

## Architecture

- [ ] Modular monolith structure present.
- [ ] Vertical slices inside modules.
- [ ] CQRS-lite contracts present.
- [ ] PostgreSQL default present.
- [ ] Schema-per-module design documented.
- [ ] Architecture tests verify module boundaries.
- [ ] Event sourcing is not default and is documented as advanced/optional.
- [ ] Module manifests are generated for business modules.

## Spec-driven development

- [ ] `specs/README.md` present.
- [ ] `specs/_template` contains spec, plan, tasks, acceptance, risks, and open-questions templates.
- [ ] At least one concrete spec exists.
- [ ] `npm run check:specs` validates specs.
- [ ] Spec workflows exist for create, review, and implement.

## AI development system

- [ ] AGENTS.md present.
- [ ] OpenQuestions.md present with required sections.
- [ ] CLAUDE.md points to AGENTS.md.
- [ ] Copilot instructions point to AGENTS.md.
- [ ] `.agents/skills` contains SKILL.md files with metadata.
- [ ] `.ai/workflows` contains workflows.
- [ ] `.ai/policies` contains risk, dependency, security, and approval policies.
- [ ] docs/open-questions-policy.md explains blocker versus inferred decisions.
- [ ] Node `.mjs` guardrail runner present.
- [ ] No duplicated `.sh`/`.ps1` guardrail logic.
- [ ] Module manifest docs and template exist.

## CI and guardrails

- [ ] `npm run check` passes.
- [ ] `npm run check:ai` passes.
- [ ] `npm run check:docs` passes.
- [ ] `npm run check:security` passes.
- [ ] `npm run check:specs` passes.
- [ ] `npm run template:smoke` passes or clearly documents unavailable SDK/package blocker.
- [ ] GitHub Actions workflows present.

## Public polish

- [ ] `docs/competitive-analysis.md` present.
- [ ] `docs/getting-started/which-profile.md` present.
- [ ] `docs/public-polish-checklist.md` present.
- [ ] README has golden path examples.
- [ ] README has profile matrix.
- [ ] README has template smoke matrix.

## Final report

- [ ] Codex reports commit list.
- [ ] Codex reports validation commands and outcomes.
- [ ] Codex reports known limitations.
- [ ] Codex reports open blockers and inferred assumptions from OpenQuestions.md.
- [ ] Codex reports specs used or updated.
- [ ] Codex reports next recommended release steps.
