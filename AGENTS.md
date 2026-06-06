# Agent Instructions for Aegis.Modulith

This file is the canonical instruction file for all AI coding agents working in this repository.

Tool-specific files such as `CLAUDE.md`, `.github/copilot-instructions.md`, IDE rules, or local config files must not duplicate architecture rules. They should point back to this file.

## Project mission

Build and maintain `Aegis.Modulith`: a pragmatic .NET modular monolith template system with CQRS-lite, vertical slices, PostgreSQL, Aspire, OpenTelemetry, optional MediatR, spec-driven workflows, module manifests, and enterprise-grade AI development guardrails.

## Read first

Before implementing anything, read:

1. `GOAL.md`
2. `OpenQuestions.md`
3. `docs/project-brief.md`
4. `docs/architecture.md`
5. `docs/cli-template-spec.md`
6. `docs/implementation-plan.md`
7. `docs/open-questions-protocol.md`
8. `docs/ai-development/spec-driven-development.md`
9. The relevant spec under `specs/`, if one exists
10. The relevant workflow under `.ai/workflows`
11. The relevant skill under `.agents/skills`

## Non-negotiable architecture rules

- API-only first.
- Modular monolith by default.
- CQRS-lite, not event sourcing by default.
- Vertical slices inside business modules.
- PostgreSQL default.
- One database with schema per module.
- One DbContext per module.
- No cross-module foreign keys.
- No cross-module EF navigation properties.
- No module may reference another module's Infrastructure.
- Domain must not depend on Infrastructure or ASP.NET Core.
- Queries must not mutate state.
- Commands must express business intent.
- Do not expose EF entities from API responses.
- Do not create generic repositories over EF Core.
- MediatR is optional, not default.
- Event sourcing requires an explicit ADR and must not be added by default.
- Generated business modules should include `module.json` manifests.

## AI development rules

- `AGENTS.md` is the canonical instruction source.
- `OpenQuestions.md` is the canonical human-question and inferred-decision ledger.
- `specs/` is the canonical spec-driven development layer for meaningful changes.
- Do not stop for clarification when a safe default exists; record the assumption under `OpenQuestions.md` and continue.
- Stop only for true blockers, document them in `OpenQuestions.md`, and continue independent work where safe.
- Codex skills live under `.agents/skills`.
- Workflows and policies live under `.ai`.
- Node `.mjs` is the single guardrail runner.
- Do not create duplicate `.sh` and `.ps1` check logic.
- CI is the final authority.
- Do not weaken tests, analyzers, security checks, or CI to make work pass.

## Human question discipline

- Use `OpenQuestions.md` as the shared human-agent decision queue.
- Record true blockers immediately.
- For low or medium risk unknowns, make a safe assumption, mark it as `inferred`, and continue.
- Do not silently make high or critical risk assumptions.
- Stop only when a blocker prevents safe implementation or validation.
- Promote durable architectural decisions from `OpenQuestions.md` into ADRs.
- Promote spec-scoped questions to root `OpenQuestions.md` when they affect public template UX, security, dependencies, licensing, CI/release, or architecture.

## Security rules

- Never commit secrets.
- Never read or modify local `.env`, key files, token stores, or machine-specific secret files.
- Never log passwords, tokens, API keys, authorization headers, or PII.
- Do not add production dependencies without checking the dependency policy.
- Do not modify auth/authz without explicit security documentation and tests.

## Build and validation commands

Prefer the repository scripts:

```bash
npm run check
npm run check:ai
npm run check:docs
npm run check:security
npm run check:specs
npm run template:smoke
```

Generated .NET solutions must pass:

```bash
dotnet restore
dotnet build -c Release
dotnet test -c Release
```

## Git rules

- Initialize git if needed.
- Use `main` as the primary branch.
- Make atomic commits after validated milestones.
- Do not push to a remote.
- Do not rewrite history unless explicitly requested.

## Required final report

Every substantial task must end with:

- What changed.
- Files or areas touched.
- Validation commands run.
- What passed and what did not.
- Git commit hashes created.
- Whether `OpenQuestions.md` changed.
- Unresolved blockers or inferred decisions.
- Known limitations.
- Open blockers from `OpenQuestions.md`.
- Inferred assumptions from `OpenQuestions.md`.
- Spec folder used or updated, if applicable.
- Recommended next step.
