# Dependency Policy

- Prefer built-in .NET and Microsoft packages first.
- Prefer stable, maintained, permissive-license packages.
- Do not add production dependencies without documenting why.
- Keep MediatR optional; do not make it mandatory.
- Run restore, build, tests, and guardrails after dependency changes.
