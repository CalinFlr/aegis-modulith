# Agent Instructions

This generated repository follows the Aegis.Modulith rules:

- API-only first.
- Modular monolith by default.
- CQRS-lite, not event sourcing by default.
- Vertical slices inside business modules.
- PostgreSQL default with one schema and DbContext per module.
- No cross-module Infrastructure references.
- No cross-module foreign keys.
- Do not expose EF entities from API responses.
- Do not create generic repositories over EF Core.
- MediatR is optional; the generated mediator mode is `AegisMediatorValue`.
- Generated modules must include `module.json` manifests.

Use `OpenQuestions.md` for blockers and inferred decisions.
