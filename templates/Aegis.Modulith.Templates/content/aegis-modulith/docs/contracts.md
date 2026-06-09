# Contracts

#if (profile == "core")
## Core Profile

Core does not generate the pro/advanced contract test project by default. It keeps the generated test surface lightweight and avoids pulling in `WebApplicationFactory`, fake-auth, inbox, and pro/advanced contract-test assets.

Core still generates API metadata and module contracts that can be extended later. Choose `--profile pro` or `--profile advanced` when you want the generated contract test foundation.
#else
## Generated Contract Tests

This profile includes `tests/Aegis.Template.ContractTests` in the solution.

These are not performance tests. No broker, external identity provider, Docker runtime, or external service is required.

The API contract tests verify semantic OpenAPI and endpoint metadata:

- the OpenAPI document can be produced;
- expected routes and HTTP methods exist;
- expected request and response content types are declared where generated endpoints declare bodies;
- expected response status codes are represented;
- the OpenAPI document includes the JWT bearer security scheme;
- protected endpoints expose named permission policy metadata;
- fake authentication markers are absent from the production API contract.

The integration event contract tests verify:

- generated integration events round-trip with `System.Text.Json`;
- integration event type/version metadata exists through `IntegrationEventContractAttribute`;
- integration contracts live under module `Contracts` folders;
- domain events and integration events remain distinct;
- the inbox sample handler consumes integration event contracts, not domain entities.

The inbox contract checks verify that `MessageId`, `IdempotencyKey`, message type, and serialized payload are represented and serializable. They do not claim broker-level exactly-once delivery.

## Updating Contracts

When you intentionally change a route, HTTP method, declared status code, request/response body, permission policy, or integration event payload, update the endpoint/event metadata and the matching contract test in the same change.

Increment integration event contract versions when a consumer-visible event payload changes incompatibly.
#endif
