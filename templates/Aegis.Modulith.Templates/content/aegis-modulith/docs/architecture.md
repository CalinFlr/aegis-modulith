# Architecture

This API uses a modular monolith structure with CQRS-lite and vertical slices.

Rules:

- One process by default.
- One PostgreSQL database.
- Schema per module.
- DbContext per module.
- No cross-module foreign keys.
- No module may reference another module's Infrastructure.
- Commands mutate state; queries do not.
- API responses use DTOs, not EF entities.
- Event sourcing is not enabled by default.

#if (profile != "core")
See [Authentication And Permissions](authentication.md) for generated JWT bearer and permission-policy scaffolding.
#endif
