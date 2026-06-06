# ADR-0013: Use Module Manifest

## Status

Accepted

## Context

Aegis.Modulith uses a modular monolith architecture. Module boundaries must be clear to humans, tests, docs, and AI agents.

## Decision

Each generated business module should include a lightweight `module.json` manifest that documents the module name, schema, owner, dependencies, public contracts, features, and boundary rules.

## Consequences

- Module ownership is explicit.
- AI agents get better local context.
- Guardrails can validate module metadata.
- Documentation and diagrams can be generated later.
- The manifest must remain metadata, not a heavy runtime framework.
