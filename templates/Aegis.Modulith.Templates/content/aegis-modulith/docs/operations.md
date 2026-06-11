# Operations

The generated API is designed to run as a single deployable process.

Operational defaults include health checks, structured logging, OpenTelemetry wiring, and PostgreSQL configuration.

For production deployment, externalize connection strings and secrets through the hosting platform rather than storing them in repository files.
