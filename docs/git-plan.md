# Git Plan

Codex must manage git deliberately.

## Initialization

If `.git` does not exist:

```bash
git init
git branch -M main
```

Do not add a remote. Do not push.

## Commit style

Use conventional commits:

```text
chore: initialize aegis modulith repository
docs: add architecture and ai development specification
docs: add spec-driven development layer
build: add node guardrail runner
feat: add dotnet template package skeleton
feat: implement core profile
feat: implement pro profile
feat: add optional mediatr support
feat: add advanced enterprise profile
feat: add taskhub reference sample
ci: validate templates and guardrails
docs: polish public repository experience
```

## Commit rules

- Commit after each validated milestone.
- Do not commit generated temp files, bin/obj, node_modules, local env files, or secrets.
- Run `git status --short` before each commit.
- Use focused commits.
- If a phase is partially complete, commit only if the repo is in a coherent state and clearly document limitations.

## Final git report

At the end, run:

```bash
git log --oneline --decorate -n 20
git status --short
```

Include the output summary in the final report.
