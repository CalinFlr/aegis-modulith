# ADR: Use Node .mjs Guardrail Runner

## Status

Accepted

## Context

`Aegis.Modulith` aims to be useful for personal projects while being polished enough for enterprise teams and open-source adoption.

## Decision

Use Node .mjs as the single cross-platform guardrail runner to avoid duplicated Bash/PowerShell logic while keeping the API runtime purely .NET.

## Consequences

- The template remains practical for everyday API projects.
- Advanced capabilities remain possible without being forced by default.
- Decisions are documented for human maintainers and AI agents.
