# ADR: Use Agent-Neutral AI Development

## Status

Accepted

## Context

`Aegis.Modulith` aims to be useful for personal projects while being polished enough for enterprise teams and open-source adoption.

## Decision

Use AGENTS.md as canonical instructions and keep tool-specific files as pointers. Store reusable Codex skills under .agents/skills and workflows/policies under .ai.

## Consequences

- The template remains practical for everyday API projects.
- Advanced capabilities remain possible without being forced by default.
- Decisions are documented for human maintainers and AI agents.
