# Guardrail Rules

These rules are validated by the Node `.mjs` guardrail runner where possible and by CI once implementation exists.

## Agent instruction rules

- AGENTS.md must exist.
- CLAUDE.md must point to AGENTS.md.
- Copilot instruction files must point to AGENTS.md and not duplicate architecture rules.
- OpenQuestions.md must exist and keep blockers visible.

## Spec rules

- Specs must exist for meaningful feature, template, workflow, or architecture changes.
- A spec folder must include `spec.md`, `plan.md`, `tasks.md`, `acceptance.md`, `risks.md`, and `open-questions.md`.
- Repository-wide spec questions must be promoted to root `OpenQuestions.md`.

## Module manifest rules

- Generated business modules should include `module.json`.
- Module manifests must be valid JSON.
- Module manifests must default cross-module database access to false.
- Module manifests must default infrastructure references to false.

## Architecture rules

- No module may reference another module's Infrastructure.
- Domain must not depend on Infrastructure.
- Queries must not mutate state.
- Event sourcing must not be introduced by default.
- MediatR must remain optional.

## Security rules

- Do not commit secrets.
- Do not weaken security checks, tests, analyzers, or CI to pass.
- Do not modify auth/authz without tests and documentation.
