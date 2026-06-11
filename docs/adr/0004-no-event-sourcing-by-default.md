# ADR: Do Not Use Event Sourcing by Default

## Status

Accepted

## Context

`Aegis.Modulith` aims to be useful for personal projects while being polished enough for enterprise teams and open-source adoption.

## Decision

Use state-based persistence by default. Keep domain events, integration events, audit, and outbox-ready flow, but do not make events the source of truth unless a specific module has an ADR.

## Consequences

- The template remains practical for everyday API projects.
- Advanced capabilities remain possible without being forced by default.
- Decisions are documented for human maintainers and AI agents.
