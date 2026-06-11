# Current Implementation Verification Review

Date: 2026-06-08

## Summary

The repository is in a buildable and guardrail-clean state for the validation paths that currently exist. `npm run check` passes, and a fresh `npm run template:smoke` successfully packs, installs, generates, restores, builds, and tests the documented smoke matrix plus item templates.

The main risk is that the current validation proves buildability more than semantic correctness. Several documented options and profile behaviors are present in template metadata but are not active in generated code. The largest gaps are:

- `--mediator mediatr` generated solutions still contain the core dispatcher path after template generation.
- `pro` and `advanced` service registrations and endpoints are stripped from generated `Program.cs`, so profile files exist but are not wired at runtime.
- `core` generated output still includes AppHost, ServiceDefaults, and Dockerfile assets that the docs describe as pro additions.
- Generated AI/guardrail/profile options are mostly not materialized beyond a few files and value replacements.
- Public polish and advanced/pro scaffolding claims are ahead of the observed implementation.

No implementation fixes were made in this run.

## Pass/Fail Table By Acceptance Area

| Acceptance area | Status | Observed evidence | Notes |
| --- | --- | --- | --- |
| Repository baseline | Pass | `git status --short --branch` showed `main`; license/docs/contributor files are present. | Existing generated artifacts under `artifacts/` are ignored by git. |
| Required docs and specs | Pass | Required docs and `specs/0001-aegis-template-core` files are present. `npm run check` passed docs/spec validation. | Several checklist statuses appear over-optimistic; see scope drift. |
| OpenQuestions discipline | Pass | `OpenQuestions.md` has required sections and existing inferred decisions. | No new true blocker or inferred decision was discovered that requires an OpenQuestions update. |
| AI development system, repository-level | Pass | `.ai/workflows`, `.ai/policies`, `.ai/guardrails`, `.ai/evals`, `.agents/skills`, `AGENTS.md`, `CLAUDE.md`, Copilot pointer, and `tools/guardrails/check.mjs` exist. | Root guardrails validate only selected presence/shape rules. |
| Guardrail scripts and CI | Pass | `npm run check` passed `ai`, `open questions`, `skills`, `workflows`, `docs`, `specs`, `module manifest template`, `ci workflows`, and `security`. Root workflows exist for CI/docs/security/specs/guardrails. | CI coverage inherits the same semantic gaps as the local smoke runner. |
| Template package and install | Pass | `npm run template:smoke` packed `Aegis.Modulith.Templates.0.1.0-alpha.1.nupkg` and installed five templates. | Package project contains a `NoWarn` entry for `NU5128`, which conflicts with the policy posture unless explicitly justified. |
| Main template smoke build/test matrix | Pass | Fresh smoke generated and tested `core-core`, `core-mediatr`, `pro-core`, `pro-mediatr`, `advanced-core`, `advanced-mediatr`, `taskhub`, and `strict-enterprise`; each generated solution built and architecture tests passed. | This validates buildability, not all selected option behavior. |
| Mediator option behavior | Fail | Generated `core-mediatr` has `AegisMediator=mediatr` and MediatR package references, but generated `DispatchingServiceCollectionExtensions.cs`, commands, and handlers contain the core dispatcher path and no active MediatR wiring. | Spec FR-004 says `--mediator core|mediatr` must generate a functional CQRS dispatch path. MediatR mode is not functionally active. |
| Profile behavior | Fail | Generated `pro-core` and `advanced-core` include profile-specific files, but generated `Program.cs` does not call `AddProProfileServices`, `MapProProfileEndpoints`, `AddAdvancedProfileServices`, or `MapAdvancedProfileEndpoints`. Generated `core-core` still includes AppHost, ServiceDefaults, and Dockerfile. | Template-time processing appears to remove C# `#if AEGIS_*` blocks before MSBuild constants can activate them. |
| TaskHub sample | Partial | Smoke verifies TaskHub builds/tests and has Projects, Tasks, Notifications, and Audit manifests. Template content includes representative commands, queries, events, and module manifests. | The sample demonstration is still thin: no dedicated TaskHub docs, only architecture tests, and pro outbox behavior is not wired because profile wiring is stripped. |
| Item templates | Partial | `aegis-module` and `aegis-worker` build; `aegis-slice` and `aegis-event` generate files. | Slice output is a standalone request/handler/route string, not a CQRS-integrated command/query slice. Event output is a generic record with a scope string, not clearly distinct domain/integration event scaffolding. Module `BillingDbContext` is a plain class, not an EF Core DbContext. |
| Architecture rules | Partial | Generated solutions have modular folders, CQRS contracts, EF Core/Npgsql, schema names, module manifests, and architecture tests. | Tests do not validate no cross-module foreign keys/navigation properties, query non-mutation, API DTO/entity separation, profile wiring, or MediatR semantics. No module migrations were observed. |
| Module manifests | Pass | Root and generated module manifest templates contain required fields and rules. Smoke checks TaskHub manifests and architecture tests validate generated module manifests. | Future guardrails should validate generated manifests beyond the template path. |
| Generated AI/guardrail options | Fail | Main template declares `--ai`, `--guardrails`, `--hooks`, `--skills`, and `--docs`, but strict-enterprise output contains only `AGENTS.md`, `OpenQuestions.md`, `specs/README.md`, docs, `.github/workflows/ci.yml`, and `lefthook.yml`; no generated `.ai`, `.agents`, `tools/guardrails`, or `package.json` were observed. | `guardrails`, `skills`, `docs`, and `license` parameters mostly have no observable file-shape effect. |
| Public polish | Partial | README has value proposition, install, golden path, profile matrix, and smoke matrix. Public polish docs exist. | `docs/public-polish-checklist.md` still has unchecked visuals, badges, and repository-health checklist items, while `docs/acceptance-criteria.md` marks public polish complete. |

## Missing Items

- Semantic smoke assertions for:
  - generated MediatR code path;
  - pro and advanced endpoint/service wiring;
  - `core` excluding pro-only assets;
  - generated AI/guardrail/skills/docs/license option outputs;
  - `guardrails off|standard|strict` behavior.
- Functional MediatR generation. Current generated MediatR variants build but still use core dispatching.
- Active pro profile wiring in generated `Program.cs`, including pro services/endpoints and rate limiter middleware.
- Active advanced profile wiring in generated `Program.cs`.
- Clear implementation or documentation reduction for pro claims: Testcontainers integration tests, fake auth test handler, HttpClient resilience defaults beyond `AddHttpClient`, and CI-ready generated guardrails.
- Clear implementation or documentation reduction for advanced claims: JWT/auth scaffolding, inbox pattern, contract tests, performance smoke tests, deployment skeleton, strict generated guardrails, and MCP-ready documentation.
- Generated enterprise AI assets if the CLI profile spec intends generated apps to include `.ai`, `.agents`, `tools/guardrails`, `package.json`, workflows, policies, skills, and evals.
- Stronger item templates for module, slice, and event scaffolding that integrate with the generated architecture.
- Public polish assets and status alignment: diagrams, screenshots, badges, and checked checklist items only after completion.

## Broken Or Incomplete Items

- Template C# preprocessor blocks are not surviving generation as intended. Generated output shows stripped `#if AEGIS_MEDIATR`, `#if AEGIS_PRO_OR_ADVANCED`, and `#if AEGIS_ADVANCED` sections even when MSBuild properties define the corresponding constants.
- `--mediator mediatr` produces package references and metadata but not active MediatR dispatching.
- `pro` and `advanced` profile files are generated but not registered or mapped in the application pipeline.
- `core` output includes AppHost, ServiceDefaults, and Dockerfile despite docs positioning these as pro additions.
- Declared template options are broader than the observable generated behavior. `skills`, `docs`, `license`, and most `guardrails` settings do not appear to affect generated files.
- Architecture tests pass but are too shallow to support all architecture checklist claims.
- Acceptance documents and spec tasks mark areas complete that are only partially implemented.

## Risks

- Users can select `--mediator mediatr` and receive a generated app that advertises MediatR mode while using core dispatching.
- Passing smoke tests may create false confidence because they do not assert option semantics.
- Profile drift can confuse users: `core` includes pro-like projects, while `pro`/`advanced` runtime behavior is inactive.
- Enterprise AI users may expect generated guardrails/workflows/skills from documented options and not receive them.
- The `NoWarn` entry in the template package project may normalize warning suppression in conflict with repository policy unless documented.
- Public readiness is overstated if checklists remain marked complete while visible polish items are unchecked.

## Suspected Scope Drift

- `docs/acceptance-criteria.md` and `specs/0001-aegis-template-core/acceptance.md` mark nearly all template, profile, guardrail, and polish criteria complete, but observed behavior shows several partial or failed semantic areas.
- The implementation appears to have optimized first for smoke build success and presence checks, while docs describe richer generated behavior.
- Profile options have become metadata/file-shape switches rather than reliable runtime behavior switches.
- Generated AI system scope is unclear: repository-level AI assets are complete, but generated-app AI assets are much smaller than the CLI profile documentation implies.
- Public polish documentation exists, but the checklist still reads as future work.

## Tests And Checks Run

- `node --version`
- `dotnet --version`
- `npm run check`
- `npm run template:smoke`
- Targeted read-only inspections of generated output under `artifacts/template-smoke/generated/**` and item template output under `artifacts/template-smoke/items/**`.

## Commands And Outputs Summarized

- `node --version`
  - Passed: `v24.16.0`
- `dotnet --version`
  - Passed: `10.0.300`
- `npm run check`
  - Exit code: 0
  - Passed checks: `ai instructions`, `open questions`, `skills`, `workflows`, `docs`, `specs`, `module manifest template`, `ci workflows`, `security`.
- `npm run template:smoke`
  - Exit code: 0
  - Packed package: `artifacts/template-smoke/packages/Aegis.Modulith.Templates.0.1.0-alpha.1.nupkg`.
  - Installed templates: `aegis-module`, `aegis-slice`, `aegis-event`, `aegis-worker`, `aegis-modulith`.
  - Generated/restored/built/tested solution variants: `core-core`, `core-mediatr`, `pro-core`, `pro-mediatr`, `advanced-core`, `advanced-mediatr`, `taskhub`, `strict-enterprise`.
  - Each generated solution build succeeded with 0 warnings and 0 errors.
  - Each generated solution architecture test run passed 4 tests.
  - Item template checks generated module/slice/event/worker files; module and worker item projects built successfully.

## Recommended Fix Order

1. Fix template conditional handling so MediatR, pro, and advanced C# blocks are generated correctly, or replace C# preprocessor blocks with template-engine conditions that produce the intended files.
2. Add semantic smoke assertions for MediatR dispatching, pro/advanced endpoints, core exclusions, and generated option-specific file sets.
3. Wire pro and advanced profile services/endpoints in generated apps and either exclude AppHost/ServiceDefaults/Dockerfile from core or update the profile docs.
4. Decide the intended generated-app scope for AI/guardrail/skills/docs/license options, then implement or narrow the CLI documentation.
5. Fill or de-scope documented pro and advanced scaffolding: Testcontainers, fake auth, resilience, JWT/auth, inbox, contract/performance tests, deployment skeleton, and strict guardrails.
6. Strengthen item templates so module/slice/event outputs integrate with the generated CQRS/module architecture.
7. Expand architecture tests and guardrails to cover cross-module FK/navigation rules, query non-mutation, DTO/entity separation, profile wiring, and generated manifests.
8. Reconcile acceptance docs and public polish checklist statuses with observed implementation.

## Blockers That Must Be Added To OpenQuestions.md

None.

The findings above are concrete implementation or documentation-alignment issues with clear fix paths. They do not require a human decision before the next implementation run, and they do not block validation from completing. `OpenQuestions.md` was not changed.

