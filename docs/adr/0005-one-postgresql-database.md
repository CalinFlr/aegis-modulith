# ADR: Use One PostgreSQL Database with Module Schemas

## Status

Accepted

## Context

`Aegis.Modulith` aims to be useful for personal projects while being polished enough for enterprise teams and open-source adoption.

## Decision

Use one PostgreSQL database for core/pro defaults. Each module owns a schema, DbContext, and migrations. No cross-module foreign keys.

## Consequences

- The template remains practical for everyday API projects.
- Advanced capabilities remain possible without being forced by default.
- Decisions are documented for human maintainers and AI agents.
