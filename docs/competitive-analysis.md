# Competitive Analysis

This document is a strategic guide for Aegis.Modulith maintainers and AI agents. It captures what strong .NET starter/template/framework repositories do well, what Aegis should learn from them, and what Aegis should intentionally avoid.

## Positioning

Aegis.Modulith should not become a generic Clean Architecture template, a full enterprise framework, or a microservices reference app.

Aegis.Modulith should be positioned as:

> The AI-ready .NET modular monolith starter for API-only systems, CQRS-lite, vertical slices, PostgreSQL, Aspire, OpenTelemetry, and enterprise development guardrails.

## Repositories to learn from

### Jason Taylor CleanArchitecture

Strengths:

- Very strong `dotnet new` template experience.
- Clear quickstart.
- Strong community recognition.
- Good profile/options style.

Apply to Aegis:

- Keep install and generation commands obvious.
- Make the README immediately useful.
- Provide a simple profile/options matrix.

Avoid:

- Becoming another generic Clean Architecture template.

### Kamil Grzybek modular-monolith-with-ddd

Strengths:

- Strong modular monolith credibility.
- Serious DDD/module boundary documentation.
- Architecture tests and decision logs.

Apply to Aegis:

- Keep module boundaries first-class.
- Explain decisions deeply.
- Include architecture tests and module contracts.

Avoid:

- Becoming only a reference app instead of a reusable template system.

### ABP Framework

Strengths:

- Enterprise vocabulary.
- Rich module ecosystem.
- Strong tooling and documentation.

Apply to Aegis:

- Use a module manifest.
- Document enterprise extension points.
- Provide clear policies and guardrails.

Avoid:

- Becoming a heavy framework or introducing lock-in.

### Orchard Core

Strengths:

- Mature modularity and multi-tenancy thinking.
- Module-oriented extensibility.

Apply to Aegis:

- Add a lightweight `module.json` manifest.
- Make module metadata useful to humans and agents.

Avoid:

- Becoming CMS-centric or plugin-framework-heavy.

### FullStackHero

Strengths:

- Strong marketing and batteries-included positioning.
- Clear value proposition.

Apply to Aegis:

- Make the value proposition concrete.
- Provide screenshots, diagrams, and golden path examples.

Avoid:

- Adding too many features by default.

### Microsoft eShop

Strengths:

- Aspire reference credibility.
- Realistic sample and diagrams.

Apply to Aegis:

- Make TaskHub realistic enough to demonstrate the architecture.
- Show Aspire and OpenTelemetry clearly.

Avoid:

- Defaulting to distributed services.

### Booking modular monolith samples

Strengths:

- Modern modular monolith, CQRS, vertical slices, event-driven examples.

Apply to Aegis:

- Provide good TaskHub bounded contexts.
- Add outbox-ready flows and representative events.

Avoid:

- Defaulting to separate read/write stores, brokers, gRPC, or MediatR.

### Elsa Workflows

Strengths:

- Mature community posture.
- Agent instruction and skills artifacts in the repository.

Apply to Aegis:

- Keep `.agents/skills` useful and real.
- Make contribution and support docs clear.

### GitHub Spec Kit

Strengths:

- Spec-driven AI development.
- Clear flow from spec to plan to tasks.

Apply to Aegis:

- Add `specs/` as a first-class workflow.
- Connect specs to OpenQuestions, ADRs, workflows, and guardrails.

## Aegis differentiators

- Template-first and reference-app-backed.
- Modular monolith first.
- CQRS-lite and vertical slices inside modules.
- PostgreSQL schema-per-module.
- Core dispatcher by default; MediatR optional.
- Event sourcing not default.
- API-only first.
- Agent-neutral AI development system.
- Node `.mjs` guardrail runner.
- OpenQuestions decision queue.
- Spec-driven development layer.
- Module manifest for humans, agents, docs, and guardrails.

## Non-goals reinforced by competitive analysis

- Do not become ABP.
- Do not become eShop.
- Do not become a frontend starter.
- Do not default to microservices.
- Do not default to event sourcing.
- Do not default to MediatR.
- Do not hide behavior behind too much framework magic.
