# ADR 0001: Use a modular monolith

## Status

Accepted

## Decision

Use a modular monolith as the default architecture.

## Consequences

- The application deploys as one process by default.
- Business capability boundaries are expressed as modules.
- Each module owns its schema, DbContext, features, and manifest.
- Cross-module communication uses contracts or documented services, not cross-module Infrastructure references.
