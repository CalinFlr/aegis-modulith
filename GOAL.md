# Codex Goal: Build Aegis.Modulith End-to-End

Use this file as the durable `/goal` objective.

## Goal statement

Build the complete `Aegis.Modulith` open-source repository from this specification pack.

Use `OpenQuestions.md` as the durable ledger for blockers, inferred decisions, non-blocking human questions, and resolved answers. Use `specs/` as the durable spec-driven development layer for meaningful features, public template changes, architecture changes, and enterprise AI workflow changes.

The finished repository must be a polished .NET template system for creating API-only modular monoliths with CQRS-lite, vertical slices, PostgreSQL, optional MediatR, Aspire, OpenTelemetry, spec-driven workflows, module manifests, and enterprise AI development guardrails.

## Product identity

- Product name: `Aegis.Modulith`
- Repository name: `aegis-modulith`
- NuGet template package: `Aegis.Modulith.Templates`
- Main template short name: `aegis-modulith`
- Module template short name: `aegis-module`
- Slice template short name: `aegis-slice`
- Event template short name: `aegis-event`
- Worker template short name: `aegis-worker`
- Sample app: `Aegis.TaskHub`
- License: Apache-2.0

## Non-negotiable architecture

- .NET 10 LTS target.
- API-only first; no UI in v0.1.
- Modular monolith, not microservices by default.
- CQRS-lite, not event sourcing by default.
- Vertical slices inside modules.
- PostgreSQL as default database.
- One database, schema per module, DbContext per module.
- No cross-module Infrastructure references.
- No cross-module foreign keys.
- No generic repository over EF Core.
- No MediatR by default; provide optional `--mediator mediatr` path.
- Node `.mjs` guardrail runner; do not duplicate check logic in `.sh` and `.ps1`.
- Agent-neutral AI system: `AGENTS.md` canonical, tool-specific files only point to it.
- `OpenQuestions.md` is the human-agent decision queue for blockers, inferred assumptions, and human-required decisions.
- `specs/` is required for meaningful feature, template, workflow, and architecture changes.
- Generated business modules should include `module.json` manifests.

## Required deliverables

Create a repository containing:

1. A `dotnet new` template package with templates:
   - `aegis-modulith`
   - `aegis-module`
   - `aegis-slice`
   - `aegis-event`
   - `aegis-worker`
   - optionally `aegis-doc`

2. A spec-driven development layer:
   - `specs/README.md`
   - `specs/_template/{spec,plan,tasks,acceptance,risks,open-questions}.md`
   - at least one concrete spec under `specs/0001-aegis-template-core`
   - workflows and skills for creating, reviewing, and implementing specs
   - guardrail validation through `npm run check:specs`

3. Profiles:
   - `--profile core`
   - `--profile pro`
   - `--profile advanced`

4. Mediator options:
   - `--mediator core`
   - `--mediator mediatr`

5. AI options:
   - `--ai none|agents|enterprise`
   - `--guardrails off|standard|strict`
   - `--hooks none|lefthook`
   - `--skills none|core|enterprise`
   - `--docs standard|full`

6. Default behavior:
   - profile: `pro`
   - mediator: `core`
   - database: `postgres`
   - sample: `none`
   - ai: `enterprise`
   - guardrails: `standard`
   - hooks: `none`
   - skills: `enterprise`
   - docs: `full`
   - license: `apache2`

7. Sample reference app:
   - `Aegis.TaskHub`, generated explicitly with `--sample taskhub`.
   - Modules: Projects, Tasks, Notifications, Audit.
   - Demonstrate commands, queries, domain events, integration events, outbox-ready flow, tests, docs, and module manifests.

8. Enterprise AI development system:
   - `AGENTS.md`
   - `OpenQuestions.md`
   - `docs/open-questions-protocol.md`
   - `CLAUDE.md` importing/pointing to `AGENTS.md`
   - `.github/copilot-instructions.md` pointing to `AGENTS.md`
   - `.agents/skills/**/SKILL.md`
   - `.ai/workflows`
   - `.ai/policies`
   - `.ai/guardrails`
   - `.ai/evals`
   - `docs/open-questions-policy.md`
   - `docs/ai-development/spec-driven-development.md`
   - `docs/ai-development/ai-pr-protocol.md`
   - `tools/guardrails/check.mjs`
   - `package.json` scripts
   - optional `lefthook.yml`

9. Module manifest support:
   - `docs/architecture/module-manifest.md`
   - `templates/module-manifest/module.json`
   - guardrail validation for manifest structure

10. Strategic/public polish docs:
   - `docs/competitive-analysis.md`
   - `docs/getting-started/which-profile.md`
   - `docs/public-polish-checklist.md`

11. CI/CD:
   - GitHub Actions for CI, security, template smoke tests, docs, guardrails, and spec validation.
   - CI must generate and build/test combinations of templates.

12. Git:
   - Initialize git if not initialized.
   - Use branch `main`.
   - Make atomic commits at milestones from `docs/git-plan.md`.
   - Do not push to a remote.

## Implementation phases

Follow `docs/implementation-plan.md`. Keep each phase small enough to validate.

## Validation

At minimum, before considering the goal complete:

```bash
npm run check
npm run check:ai
npm run check:docs
npm run check:security
npm run check:specs
npm run template:smoke
```

Generated .NET solutions in the smoke matrix must run:

```bash
dotnet restore
dotnet build -c Release
dotnet test -c Release
```

## Completion criteria

The goal is complete only when:

- The repository builds.
- The template package builds.
- The smoke matrix generates at least core/pro/advanced plus mediator variants.
- Generated solutions build and test.
- AI guardrails pass.
- Spec guardrails pass.
- `OpenQuestions.md` exists and any remaining blockers are reported.
- Docs are present and linked.
- Specs are present and validated.
- Module manifest guidance and template are present.
- ADRs are present.
- Git history contains milestone commits.
- Final report lists files changed, commits, validation commands, known limitations, open blockers, inferred assumptions, spec folders used, and next recommended work.

## Open questions protocol

Before stopping for clarification, check whether a safe default exists in `AGENTS.md`, specs, docs, ADRs, or this goal. If a safe default exists, continue and record the assumption under `OpenQuestions.md` as an inferred decision.

Use `OpenQuestions.md` for:

- true blockers,
- non-blocking decisions needed from humans,
- inferred decisions,
- follow-up questions,
- resolved answers.

## Stop conditions

Stop and report with evidence if:

- A required SDK/runtime is not available.
- A package or dependency cannot be resolved.
- Template generation cannot be validated.
- A licensing conflict blocks implementation.
- The workspace does not allow git initialization.

When blocked, do not pretend success. Leave the repo in the best validated state and document exactly what remains in both the final report and `OpenQuestions.md`.
