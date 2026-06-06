# Risks

## Technical risks

- Risk: Too many template option combinations become hard to validate.
  - Impact: Broken generated solutions.
  - Mitigation: Template smoke matrix in CI.

- Risk: Node guardrails become too large.
  - Impact: Maintainers treat them as a second product.
  - Mitigation: Keep Node limited to automation and AI/docs/spec checks.

## Security risks

- Risk: AI agents modify auth, secrets, or CI without review.
  - Impact: Security regression.
  - Mitigation: Policies, OpenQuestions.md, strict guardrails, PR checklist, and CI.

## Licensing risks

- Risk: Optional MediatR mode creates licensing friction.
  - Impact: Enterprise users hesitate.
  - Mitigation: Core dispatcher default; MediatR optional and documented.

## Adoption risks

- Risk: Project appears too complex for personal APIs.
  - Impact: Fewer users try it.
  - Mitigation: Clear `core`, `pro`, and `advanced` profiles; pro default with good docs.
