# ADR: Do Not Use MediatR by Default

## Status

Accepted

## Context

`Aegis.Modulith` aims to be useful for personal projects while being polished enough for enterprise teams and open-source adoption.

## Decision

Use an internal core dispatcher by default to avoid mandatory dependency/licensing friction and keep flow explicit. Provide MediatR as an optional template parameter.

## Consequences

- The template remains practical for everyday API projects.
- Advanced capabilities remain possible without being forced by default.
- Decisions are documented for human maintainers and AI agents.
