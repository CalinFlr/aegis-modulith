# ADR-0012: Use Spec-Driven Development

## Status

Accepted

## Context

Aegis.Modulith is intended to be built and maintained with AI coding agents. Long-running AI work needs more structure than a single prompt or a generic issue description.

## Decision

Add a `specs/` folder with a repeatable flow:

```text
spec -> plan -> tasks -> acceptance -> risks -> open questions
```

Use specs for meaningful product work, template features, architecture changes, and larger AI-assisted implementation tasks.

## Consequences

- Agents have a durable implementation contract.
- Humans can review scope before implementation.
- Open questions and inferred decisions become visible.
- ADRs remain focused on durable architectural decisions.
- The repository is easier to audit and evolve.
