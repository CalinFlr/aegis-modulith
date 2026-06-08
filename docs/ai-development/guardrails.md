# Guardrails

Guardrails are deterministic checks run through Node `.mjs`.

Repository guardrails use `tools/guardrails/check.mjs`. Generated apps materialize the guardrail runner only when `--guardrails standard` or `--guardrails strict` is selected.

Generated behavior:

- `off`: no Node guardrail runner, no guardrail npm scripts, and no guardrail CI step.
- `standard`: runner, npm scripts, and generated CI wiring for AI/docs/security/spec/skill/workflow checks appropriate to the selected output.
- `strict`: standard mode plus strict policies, strict rules, sensitive-file checks, instruction consistency checks, and skills/workflows/specs shape checks.

Do not duplicate guardrail logic in `.sh` or `.ps1` files.
