# Agent Operating Model

`AGENTS.md` is the canonical instruction source for repository and generated-app agents.

Tool-specific files such as `CLAUDE.md` and `.github/copilot-instructions.md` must point to `AGENTS.md` instead of duplicating architecture rules.

Agents should use:

- `OpenQuestions.md` for blockers, inferred decisions, and human-visible assumptions;
- `specs/` for meaningful feature, template, workflow, and architecture work;
- `.ai/workflows` for repeatable implementation procedures;
- `.agents/skills` for task-specific operating instructions.
