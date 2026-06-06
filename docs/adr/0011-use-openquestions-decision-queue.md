# ADR-0011: Use OpenQuestions.md as the Human-Agent Decision Queue

## Status

Accepted

## Context

Aegis.Modulith is intended to be built and maintained with AI coding agents. Agents can make progress quickly, but they can also hide assumptions, ask too many non-blocking questions, or stop work unnecessarily.

The project needs a lightweight mechanism for blockers, inferred decisions, and human-required questions.

## Decision

Use `OpenQuestions.md` at the repository root as the shared human-agent decision queue.

Agents must record blockers, inferred decisions, and human-required questions there. They should continue with safe documented defaults for low and medium risk questions, but stop for high or critical blockers.

## Consequences

- Human maintainers can review uncertainty in one place.
- Agents can continue work without repeatedly interrupting for non-blocking decisions.
- High-risk uncertainty becomes visible and auditable.
- Durable decisions can be promoted from `OpenQuestions.md` into ADRs.
- The final report can list remaining blockers and inferred assumptions.
