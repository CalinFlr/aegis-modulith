# Security Policy for Agents

Agents must not:

- Read or modify `.env` files.
- Commit secrets.
- Log passwords, tokens, authorization headers, API keys, or PII.
- Disable security checks.
- Weaken authentication or authorization.
- Add wildcard CORS in pro/advanced profiles without ADR.
- Change CI to hide failures.

Agents must:

- Use fake auth in tests.
- Keep security docs updated when behavior changes.
- Require human review for auth/authz, secrets, crypto, or deployment changes.
