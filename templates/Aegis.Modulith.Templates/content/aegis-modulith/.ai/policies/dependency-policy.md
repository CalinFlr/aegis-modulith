# Dependency Policy

- Prefer Microsoft/.NET built-in features first.
- Prefer stable, maintained, permissive-license packages.
- Use Central Package Management.
- Do not add production dependencies without justification.
- Run NuGet audit and template smoke tests after dependency changes.
- MediatR is optional only; do not make it mandatory.
- Avoid adding packages for trivial abstractions.
