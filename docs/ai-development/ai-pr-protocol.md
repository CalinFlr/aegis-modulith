# AI Pull Request Protocol

This protocol applies when an AI agent generates, modifies, or reviews code.

## Required summary

Every AI-assisted PR or final report must include:

- What changed.
- Why it changed.
- Files and areas touched.
- Specs updated.
- OpenQuestions updated.
- ADRs added or changed.
- Tests and checks run.
- What passed.
- What failed.
- Remaining blockers.
- Inferred assumptions.
- Human approvals required.

## Required checks

For most code changes:

```bash
npm run check
npm run template:smoke
```

Generated .NET solutions must run:

```bash
dotnet restore
dotnet build -c Release
dotnet test -c Release
```

## Human approval required

Human approval is required before merging or publishing changes involving:

- Authentication.
- Authorization.
- Secrets.
- Production dependencies.
- Public package publishing.
- CI/CD bypasses.
- Database migrations.
- Public API breaking changes.
- License changes.
- Security policies.
- Destructive commands.

## Prohibited behavior

Agents must not:

- Hide failed checks.
- Disable tests to pass CI.
- Weaken analyzers.
- Modify `.env` or secret files.
- Add production dependencies without documenting the reason.
- Make high-risk assumptions silently.
