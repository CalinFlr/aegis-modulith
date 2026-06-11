# ADR: Use CQRS-lite

## Status

Accepted

## Context

`Aegis.Modulith` aims to be useful for personal projects while being polished enough for enterprise teams and open-source adoption.

## Decision

Separate commands and queries at the application/use-case level while using the same PostgreSQL database by default. Do not introduce separate read/write stores unless an ADR justifies the complexity.

## Consequences

- The template remains practical for everyday API projects.
- Advanced capabilities remain possible without being forced by default.
- Decisions are documented for human maintainers and AI agents.
