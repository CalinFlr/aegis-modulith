# Contracts

P1D-3A adds generated contract test foundations for API and integration-contract drift in generated `pro` and `advanced` outputs.

These are not performance tests. They do not require Docker, a broker, an external identity provider, a real JWT issuer, or external services.

## Generated Contract Tests

Generated `pro` and `advanced` solutions include `tests/<App>.ContractTests` in the solution. The project runs with `dotnet test -c Release` and uses semantic assertions instead of full OpenAPI snapshots.

The API contract tests verify:

- the OpenAPI document can be produced;
- expected routes and HTTP methods exist;
- expected request and response content types are declared where the generated endpoints declare bodies;
- expected response status codes are represented where generated endpoints declare them;
- the OpenAPI document includes a JWT bearer security scheme for generated auth profiles;
- protected endpoints expose authorization metadata with named permission policies;
- fake authentication markers are absent from the production API contract;
- starter endpoints and TaskHub endpoints are represented.

The integration event contract tests verify:

- integration events serialize and round-trip with `System.Text.Json`;
- each integration event has lightweight `IntegrationEventContractAttribute` type/version metadata;
- integration event contracts live under module `Contracts` folders;
- domain events and integration events remain distinct;
- the inbox sample handler consumes integration event contracts and contract metadata, not domain entities;
- inbox `MessageId`, `IdempotencyKey`, message type, and payload contracts are serializable.

## Updating Contracts

When intentionally changing an API route, method, status code, permission policy, or integration event shape, update the generated endpoint metadata and the matching contract test in the same change.

When changing integration event compatibility, update the `IntegrationEventContractAttribute` type/version metadata deliberately. Increment the version when a consumer-visible event payload changes incompatibly.

Do not use these tests to claim broker-level exactly-once delivery. The inbox contract covers local idempotency fields and serialization boundaries only.

## Profile Behavior

`core` remains lightweight and does not include the generated pro/advanced contract test project by default. Its generated docs explain the exclusion.

`pro` and `advanced` include the contract test project and run it as part of generated `dotnet test`.
