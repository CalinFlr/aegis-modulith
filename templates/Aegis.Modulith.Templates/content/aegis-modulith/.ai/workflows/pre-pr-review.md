# Workflow: Pre-PR Review

## Purpose

Review an AI-assisted change before merge.

## Steps

1. Summarize changed files and behavior.
2. Check architecture rules.
3. Check docs and OpenQuestions updates.
4. Run required validation.
5. Report residual risk.

## Required validation

```bash
npm run check
dotnet test -c Release
```

## Human approval

Required for high and critical risk areas.
