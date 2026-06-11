# ADR: Use Vertical Slices inside Modules

## Status

Accepted

## Context

`Aegis.Modulith` aims to be useful for personal projects while being polished enough for enterprise teams and open-source adoption.

## Decision

Organize features by use case inside business modules. Each slice owns endpoint, request, handler, response, validation, and tests.

## Consequences

- The template remains practical for everyday API projects.
- Advanced capabilities remain possible without being forced by default.
- Decisions are documented for human maintainers and AI agents.
