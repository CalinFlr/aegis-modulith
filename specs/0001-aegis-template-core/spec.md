# SPEC-0001: Aegis.Modulith template system core

## Status

Draft

## Problem

.NET developers need a pragmatic starter for API-only modular monoliths that is useful in personal projects, polished enough for open-source adoption, and safe enough for AI-assisted enterprise development.

Existing templates usually optimize for one axis: Clean Architecture, full framework, reference app, or generic AI workflow. Aegis.Modulith should combine a .NET modular monolith starter with AI-agent guardrails and spec-driven development without becoming a heavy framework.

## Goals

- Build a `dotnet new` template package called `Aegis.Modulith.Templates`.
- Provide a main template named `aegis-modulith`.
- Provide item templates for modules, slices, events, and workers.
- Support `core`, `pro`, and `advanced` profiles.
- Use CQRS-lite and vertical slices inside business modules.
- Use PostgreSQL with schema-per-module by default.
- Use a core dispatcher by default and MediatR only as an optional mode.
- Include enterprise AI development assets: `AGENTS.md`, skills, workflows, guardrails, policies, evals, and `OpenQuestions.md`.
- Include a spec-driven development layer under `specs/`.

## Non-goals

- Do not create a full application framework.
- Do not default to microservices.
- Do not default to event sourcing.
- Do not default to MediatR.
- Do not default to separate read/write databases.
- Do not include a UI in v0.1.
- Do not require Node at runtime for generated APIs.

## Users

- Developers starting a new .NET API.
- Maintainers building an open-source template project.
- Enterprise teams that need guardrails for AI-assisted development.
- AI coding agents implementing and maintaining the repository.

## Requirements

### Functional requirements

- FR-001: The template package must build as a NuGet template package.
- FR-002: `dotnet new aegis-modulith` must generate a buildable .NET solution.
- FR-003: `--profile core|pro|advanced` must alter generated assets predictably.
- FR-004: `--mediator core|mediatr` must generate a functional CQRS dispatch path.
- FR-005: `--sample taskhub` must generate a reference app with Projects, Tasks, Notifications, and Audit modules.
- FR-006: `aegis-module` must add a module with a module manifest.
- FR-007: `aegis-slice` must add command/query vertical slices.
- FR-008: `npm run check` must validate AI/docs/spec/manifest guardrails.

### Non-functional requirements

- NFR-001: Generated code must be readable and low magic.
- NFR-002: Guardrails must be cross-platform through Node `.mjs`.
- NFR-003: The generated API must not require Node at runtime.
- NFR-004: Architecture boundaries must be testable.
- NFR-005: Documentation must explain decisions and trade-offs.

## Acceptance summary

The spec is complete when Codex creates a repository that builds, generates template variants, validates generated solutions, passes guardrails, includes docs/ADRs/specs, and commits milestone changes.

## Open questions policy

Use `specs/0001-aegis-template-core/open-questions.md` for local unknowns. Promote project-wide decisions to root `OpenQuestions.md`.
