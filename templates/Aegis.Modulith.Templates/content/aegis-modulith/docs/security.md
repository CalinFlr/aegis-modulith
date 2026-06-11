# Security

Do not commit secrets, local `.env` files, private keys, tokens, or machine-specific credentials.

Generated guardrails check for common sensitive file patterns, but CI is not a substitute for review.

Authentication and authorization changes should be documented and tested before merge.
