# Open Questions

This file is the shared decision queue between AI agents and human maintainers.

It exists so agents can continue implementation with explicit assumptions instead of silently guessing or repeatedly stopping for non-blocking clarification.

## Rules

1. Do not use this file for ordinary TODOs. Use it only for questions, blockers, assumptions, and decisions that need human visibility.
2. Do not stop work for low-risk unknowns. Make a reasonable assumption, mark it as `inferred`, and continue.
3. Stop work only for true blockers: security, licensing, irreversible architecture choices, missing runtimes, missing package access, data loss risk, or anything that prevents validation.
4. Every entry must include a proposed default and the impact if the human chooses differently.
5. When a human answers, move the entry to `Answered or decided` and update the related docs, ADRs, code, or tests.
6. Do not delete resolved entries until the end of a milestone; preserve decision traceability.
7. Spec-local questions live in `specs/<id>/open-questions.md`; promote them here when they affect public architecture, security, licensing, CI/CD, template UX, dependencies, or release.

## Status values

- `open`: needs human answer but does not block current work.
- `blocker`: current milestone cannot be completed safely without a human answer.
- `inferred`: agent made a reasonable assumption and continued.
- `answered`: human answered, implementation not yet updated.
- `decided`: answer has been applied to code/docs/tests.
- `deferred`: intentionally postponed.

## Risk values

- `low`: naming, wording, minor docs, local convention.
- `medium`: public API shape, template UX, generated folder structure.
- `high`: security, licensing, dependencies, auth/authz, database migrations, public contracts.
- `critical`: release, package publishing, destructive commands, data loss, CI bypass, license change.

## Open or inferred questions

<!--
Use this template:

### Q-YYYYMMDD-001: Short question title

- Status: open | blocker | inferred | answered | decided | deferred
- Risk: low | medium | high | critical
- Owner: human | agent | maintainer
- Source: user request | implementation | validation | dependency | security review
- Affected areas: files, modules, templates, docs, CI, packages
- Question: What needs a decision?
- Context: Why did this come up?
- Proposed default: What should the agent do if no answer is available?
- Impact if different: What changes if the human chooses another path?
- Current action: continue | stop | implement default | document only
- Created: YYYY-MM-DD
- Resolved: YYYY-MM-DD or N/A
-->

### Q-20260606-001: Confirm final public project name before publish

- Status: inferred
- Risk: medium
- Owner: human
- Source: naming decision
- Affected areas: NuGet package, GitHub repository, namespaces, README, docs
- Question: Should the final public name remain `Aegis.Modulith`?
- Context: `Aegis.Modulith` was selected as the working name because it communicates protection/guardrails and keeps `Modulith` for discoverability.
- Proposed default: Continue implementation with `Aegis.Modulith`.
- Impact if different: Rename package IDs, namespaces, template names, docs, GitHub repo metadata, and sample names before public release.
- Current action: continue
- Created: 2026-06-06
- Resolved: N/A

### Q-20260606-002: Adopt spec-driven development layer

- Status: inferred
- Risk: medium
- Owner: human
- Source: competitive review and AI development design
- Affected areas: specs, workflows, skills, guardrails, docs, template strategy
- Question: Should Aegis.Modulith include a spec-driven development layer inspired by modern AI-assisted development workflows?
- Context: Competitive review showed that spec-driven workflows are useful for long-running agent work and reduce silent assumptions.
- Proposed default: Include `specs/` with templates and require specs for medium/high-risk work.
- Impact if different: Remove specs, related workflows, guardrails, and docs; rely only on AGENTS.md, OpenQuestions.md, and ADRs.
- Current action: continue
- Created: 2026-06-06
- Resolved: N/A

### Q-20260606-003: Include module.json manifests in generated modules

- Status: inferred
- Risk: medium
- Owner: human
- Source: module boundary and AI-agent context design
- Affected areas: aegis-module template, docs, guardrails, architecture tests, generated modules
- Question: Should each generated module include a lightweight `module.json` manifest?
- Context: Module metadata helps humans and AI agents understand ownership, schema, dependencies, public contracts, features, and boundary rules.
- Proposed default: Include `module.json` manifests as metadata only, not as a heavy runtime framework.
- Impact if different: Remove manifest generation and rely on docs/conventions/tests only.
- Current action: continue
- Created: 2026-06-06
- Resolved: N/A

### Q-20260606-004: Pin optional MediatR to Apache-2.0 package line

- Status: inferred
- Risk: high
- Owner: human
- Source: dependency and licensing review
- Affected areas: template package, generated Directory.Packages.props, mediator option docs, smoke tests
- Question: Should the optional `--mediator mediatr` path pin MediatR to version `12.5.0` instead of the current NuGet latest line?
- Context: NuGet resolved MediatR `14.1.0`, whose package metadata requires license acceptance and points to RPL/commercial terms. MediatR `12.5.0` restores on .NET 10 and declares `Apache-2.0` license metadata.
- Proposed default: Pin optional MediatR to `12.5.0` and keep the core dispatcher as the default.
- Impact if different: Moving to the newer MediatR line would require explicit maintainer license approval and updated generated documentation.
- Current action: implement default
- Created: 2026-06-06
- Resolved: N/A

### Q-20260608-001: Define the generated core AI skill subset

- Status: inferred
- Risk: medium
- Owner: human
- Source: P1A generated AI option materialization
- Affected areas: templates, generated `.agents/skills`, generated AI docs, smoke assertions
- Question: Which skills should `--skills core` include in generated enterprise AI output?
- Context: The CLI documents `--skills none|core|enterprise`, but the previous implementation did not materialize a generated skill set. P1A needed a concrete, testable core subset.
- Proposed default: Treat `core` as the skills needed for basic docs, architecture, module, vertical-slice, module-manifest, and spec workflows: `docs-writer`, `dotnet-architecture-review`, `dotnet-module`, `dotnet-vertical-slice`, `module-manifest`, and `spec-driven-feature`.
- Impact if different: Update generated `.agents/skills`, generated AI docs, and smoke assertions to match the maintainer-selected core subset.
- Current action: implement default
- Created: 2026-06-08
- Resolved: N/A

### Q-20260609-001: Prefer drop-in item templates over automatic solution mutation

- Status: inferred
- Risk: medium
- Owner: human
- Source: P1B item template implementation
- Affected areas: aegis-module, aegis-slice, aegis-event, aegis-worker, smoke tests, README, CLI docs
- Question: Should item templates automatically edit generated solution files, or should they generate buildable drop-in outputs with explicit namespace/project options?
- Context: `dotnet new` item templates are reliable at generating files but are not a good fit for safely mutating `.sln`, project references, or `Program.cs` across all generated profile/mediator combinations.
- Proposed default: Generate item outputs under the generated modules project using explicit `--rootNamespace`, `--buildingBlocksNamespace`, `--buildingBlocksProject`, and `--mediator` options, then document the expected add/build flow instead of promising automatic solution edits.
- Impact if different: Automatic insertion would require a separate tool or post-action flow, broader smoke coverage, and more failure handling around solution/project mutation.
- Current action: implement default
- Created: 2026-06-09
- Resolved: N/A

### Q-20260609-002: Keep generated Testcontainers tests opt-in

- Status: inferred
- Risk: medium
- Owner: human
- Source: P1D-1 implementation
- Affected areas: templates, generated integration tests, smoke tests, docs, CI
- Question: Should generated Testcontainers PostgreSQL integration tests run during the default template smoke matrix?
- Context: Testcontainers requires local Docker or a compatible container runtime. Docker availability varies across developer machines and CI agents, and the P1D-1 goal says default smoke must not require Docker.
- Proposed default: Generate the Docker-backed tests for pro and advanced profiles, but skip them by default unless `AEGIS_RUN_TESTCONTAINERS=true` is set.
- Impact if different: Default smoke and generated `dotnet test` could fail on machines without Docker; CI would need explicit Docker setup and separate runtime validation.
- Current action: implement default
- Created: 2026-06-09
- Resolved: N/A

### Q-20260609-003: Keep fake authentication test-only until auth scaffolding

- Status: inferred
- Risk: high
- Owner: human
- Source: P1D-1 implementation
- Affected areas: generated integration tests, auth test infrastructure, docs, future P1D-2 auth scaffolding
- Question: Should fake authentication be available anywhere outside generated test projects before real JWT/auth scaffolding exists?
- Context: P1D-1 needs a fake auth test foundation, but P1D-2 JWT/auth scaffolding, permissions, and authorization policy work are explicitly out of scope.
- Proposed default: Keep fake authentication only under generated integration test projects and enable it only through the test `WebApplicationFactory`.
- Impact if different: Production code would need real auth/authz design, documentation, and tests from P1D-2 before fake or real schemes could be safely exposed.
- Current action: implement default
- Created: 2026-06-09
- Resolved: N/A

### Q-20260609-004: Use JWT bearer scaffold with claim-based permissions

- Status: open
- Risk: high
- Owner: human
- Source: P1D-2A implementation
- Affected areas: generated pro/advanced APIs, generated integration tests, auth docs, smoke assertions
- Question: Should generated auth use JWT bearer validation with user-supplied issuer/audience/signing key and claim-based permission policies, rather than bundling an identity provider or database-backed user system?
- Context: P1D-2A requires practical auth scaffolding, not a full identity provider integration, login flow, user database, or RBAC/ABAC engine.
- Proposed default: Generate JWT bearer scaffolding for pro/advanced, reject tokens when required JWT settings are missing, represent permissions as claims registered as named policies, and keep fake auth only in integration tests.
- Impact if different: A bundled identity provider, database-backed users, or full role/permission engine would require a separate security design, additional dependencies, broader docs, and more validation beyond P1D-2A.
- Current action: implement default behind PR review, CI, security guardrail, and explicit maintainer merge gate before release.
- Created: 2026-06-09
- Resolved: N/A

### Q-20260609-005: Use database uniqueness for inbox idempotency

- Status: inferred
- Risk: medium
- Owner: human
- Source: P1D-2B implementation
- Affected areas: templates, generated pro/advanced APIs, generated tests, docs, smoke assertions
- Question: Should the generated inbox pattern rely on database uniqueness for idempotency instead of claiming broker-level exactly-once delivery?
- Context: P1D-2B requires an inbox/idempotency foundation without introducing a message broker dependency or a larger distributed-lock framework.
- Proposed default: Use unique indexes on `MessageId` and `IdempotencyKey`, return accepted/duplicate/already-processed results from the inbox store, and document broker-level exactly-once as out of scope.
- Impact if different: A broker-specific or distributed-lock approach would require new dependencies, provider-specific docs, broader integration tests, and a separate architecture decision.
- Current action: implement default
- Created: 2026-06-09
- Resolved: N/A

### Q-20260609-006: Omit broker integration from inbox scaffold

- Status: inferred
- Risk: medium
- Owner: human
- Source: P1D-2B implementation
- Affected areas: templates, generated pro/advanced APIs, docs, smoke assertions
- Question: Should generated inbox scaffolding include a concrete broker consumer?
- Context: The goal requires a practical inbox foundation but explicitly forbids introducing a message broker dependency.
- Proposed default: Generate `IInboxStore`, `InboxProcessor`, handler dispatch, and a sample handler, and document where a broker, webhook, queue consumer, or import endpoint would call `AcceptAsync`.
- Impact if different: Adding a concrete broker would add dependency, configuration, runtime, security, and test surface beyond P1D-2B.
- Current action: implement default
- Created: 2026-06-09
- Resolved: N/A

### Q-20260609-007: Generate active inbox only for pro and advanced

- Status: inferred
- Risk: medium
- Owner: human
- Source: P1D-2B implementation
- Affected areas: profile behavior, generated docs, generated tests, smoke assertions
- Question: Should the core profile include any active inbox infrastructure?
- Context: The goal says `core` should remain lightweight and should not include inbox infrastructure by default.
- Proposed default: Exclude active inbox code and generated inbox tests from `core`; include profile-accurate messaging docs that point users to `pro` or `advanced` for inbox support.
- Impact if different: Core would become heavier and would need additional persistence and test package assertions.
- Current action: implement default
- Created: 2026-06-09
- Resolved: N/A

### Q-20260609-008: Use semantic contract assertions instead of full OpenAPI snapshots

- Status: inferred
- Risk: medium
- Owner: human
- Source: P1D-3A implementation
- Affected areas: templates, generated contract tests, smoke tests, docs
- Question: Should generated API contract tests use semantic OpenAPI and endpoint metadata assertions instead of full OpenAPI document snapshots?
- Context: Full OpenAPI snapshots would be brittle for harmless ordering and formatting changes. P1D-3A asks to prefer semantic assertions.
- Proposed default: Assert routes, methods, declared status codes, declared content types, JWT bearer security scheme, and authorization metadata semantically.
- Impact if different: Snapshot testing would need normalization rules and broader documentation to prevent noisy failures.
- Current action: implement default
- Created: 2026-06-09
- Resolved: N/A

### Q-20260609-009: Add lightweight integration event type/version metadata

- Status: inferred
- Risk: medium
- Owner: human
- Source: P1D-3A implementation
- Affected areas: BuildingBlocks events, module contracts, item event template, inbox handler, generated contract tests, docs
- Question: Should generated integration events carry explicit type/version metadata for contract tests?
- Context: Existing integration events had `Id` and `OccurredAtUtc`, but no stable contract type/version metadata for drift checks.
- Proposed default: Add `IntegrationEventContractAttribute` with a stable type string and positive integer version, and use it in inbox handler message-type matching.
- Impact if different: Tests would need to infer event names from CLR names, making contract identity less transparent for consumers.
- Current action: implement default
- Created: 2026-06-09
- Resolved: N/A

### Q-20260609-010: Generate contract test foundation only for pro and advanced

- Status: inferred
- Risk: medium
- Owner: human
- Source: P1D-3A implementation
- Affected areas: profile behavior, generated solution, generated docs, smoke assertions
- Question: Should core include the generated pro/advanced contract test project?
- Context: P1D-3A primarily applies to pro and advanced, and core must remain lightweight.
- Proposed default: Exclude `tests/<App>.ContractTests` from core and document the exclusion in generated `docs/contracts.md`.
- Impact if different: Core would pull in additional test infrastructure and a larger default solution surface.
- Current action: implement default
- Created: 2026-06-09
- Resolved: N/A

### Q-20260610-001: Generate diagnostic performance smoke tests for pro and advanced only

- Status: inferred
- Risk: medium
- Owner: human
- Source: P1D-3B implementation
- Affected areas: templates, generated tests, smoke tests, docs, CI
- Question: Should generated performance checks be diagnostic smoke tests with loose thresholds, generated only for pro and advanced profiles?
- Context: P1D-3B requires generated performance smoke tests that are deterministic enough for starter projects while explicitly avoiding a full benchmark suite, load testing, Docker, brokers, external identity providers, external services, or production performance certification.
- Proposed default: Generate `tests/<App>.PerformanceSmokeTests` for pro and advanced only, run them with default `dotnet test`, use `Stopwatch`, warm-up requests, sample diagnostics, EF InMemory, test-local fake auth, and intentionally loose named thresholds.
- Impact if different: Core would gain a heavier default test surface, or stricter benchmark-style timing could make generated validation flaky across developer machines and CI agents.
- Current action: implement default
- Created: 2026-06-10
- Resolved: N/A

### Q-20260610-002: Keep deployment skeleton provider-neutral and non-deploying by default

- Status: inferred
- Risk: medium
- Owner: human
- Source: P1D-4 implementation
- Affected areas: templates, generated Docker assets, generated CI, generated deployment docs, smoke tests
- Question: Should generated pro and advanced deployment scaffolding stay provider-neutral and avoid deploying by default?
- Context: P1D-4 requires practical deployment scaffolding without forcing a cloud provider, registry, Kubernetes cluster, Docker daemon, live database, broker, identity provider, or deployment secrets during default validation.
- Proposed default: Generate container/config examples, local API plus PostgreSQL compose scaffolding, a CI container build job, and a manually gated deployment placeholder that performs no deployment until the user configures registry and provider-specific steps.
- Impact if different: A cloud-specific deployment path would require provider credentials, secret-store choices, registry naming, broader security review, and provider-specific validation beyond P1D-4.
- Current action: implement default
- Created: 2026-06-10
- Resolved: N/A

## Blockers

No known blockers at pack creation time.

## Answered or decided

No answered questions yet.
