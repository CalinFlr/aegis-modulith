# Enterprise AI Development System

## Principle

Instructions guide agents. Guardrails verify behavior. CI is the final authority.

## Layers

| Layer | Path | Purpose |
|---|---|---|
| Canonical instructions | `AGENTS.md` | Persistent repository rules |
| Codex skills | `.agents/skills` | Reusable workflows and domain capabilities |
| Workflows | `.ai/workflows` | End-to-end procedures |
| Policies | `.ai/policies` | Risk, approval, dependency, and security rules |
| Guardrails | `.ai/guardrails` + `tools/guardrails` | Deterministic checks |
| Evals | `.ai/evals` | Scenarios to test agent behavior |
| Tool adapters | `CLAUDE.md`, `.github/copilot-instructions.md` | Pointers to AGENTS.md |

## Required behavior

Agents must:

- Read AGENTS.md first.
- Use relevant skills/workflows.
- Avoid secrets.
- Avoid weakening tests or CI.
- Run validation.
- Report what passed and what failed.
- Ask for or document approval requirements for high-risk changes.

## High-risk actions

Human approval is required before merge for:

- Auth/authz changes.
- Database migrations.
- New production dependencies.
- Public API breaking changes.
- CI/release pipeline changes.
- License changes.
- Security policy changes.
- Secret handling.
- Deployment/publishing.
