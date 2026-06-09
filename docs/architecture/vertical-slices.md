# Vertical Slices

Generated modules organize use cases as vertical slices under the module that owns the behavior.

## Rules

- Business use cases live under `Modules/<ModuleName>/Features/<SliceName>`.
- Command slices include a command, command handler, response, and endpoint mapping or module endpoint delegation.
- Query slices include a query, query handler, response projection, and endpoint mapping or module endpoint delegation.
- Slice request and response records are separate from EF/domain entities.
- Slice handlers may use the owning module infrastructure, but they must not reach into another module's Infrastructure namespace.
- Endpoints delegate to command/query dispatchers or handlers and must not perform persistence directly.

## Current Generated Shape

Starter and TaskHub modules keep route mapping in the module composition class and keep command/query records and handlers in `Features/<SliceName>`.

Item-template slices generate endpoint mapping helpers directly under `Features/<SliceName>`.

Both shapes follow the same rule: the feature folder is the home for use-case request, handler, response, validation, and endpoint code.

