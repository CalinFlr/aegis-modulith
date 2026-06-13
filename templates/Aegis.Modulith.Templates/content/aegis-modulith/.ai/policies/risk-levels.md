# AI Agent Risk Levels

## Low risk

- Add or improve documentation.
- Add a unit test.
- Refactor private code without behavior change.

Agent may proceed and validate.

## Medium risk

- Add command/query slice.
- Add integration test.
- Modify internal API behavior.

Agent may implement, must run relevant checks, and must summarize risk.

## High risk

- Add production dependency.
- Add EF Core migration.
- Modify public API contract.
- Modify module boundaries.
- Add background worker.
- Add external service integration.

Human review is required before merge.

## Critical risk

- Modify authentication or authorization.
- Handle secrets or encryption.
- Change CI/release pipeline.
- Publish NuGet package or container image.
- Delete data.
- Change license.

Explicit human approval is required before action or merge.
