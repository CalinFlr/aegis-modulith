# ADR: Use Modular Monolith

## Status

Accepted

## Context

`Aegis.Modulith` aims to be useful for personal projects while being polished enough for enterprise teams and open-source adoption.

## Decision

Use a modular monolith by default. Deploy as one process while enforcing module boundaries internally. This keeps local development and deployment simple while avoiding a tangled monolith.

## Consequences

- The template remains practical for everyday API projects.
- Advanced capabilities remain possible without being forced by default.
- Decisions are documented for human maintainers and AI agents.
