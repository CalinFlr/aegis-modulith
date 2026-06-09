# CQRS-Lite

Generated code uses CQRS at the use-case level without separate read/write databases or event sourcing.

## Rules

- Commands express business intent and implement `ICommand<TResponse>`.
- Queries express read intent and implement `IQuery<TResponse>`.
- Command handlers implement `ICommandHandler<TCommand, TResponse>`.
- Query handlers implement `IQueryHandler<TQuery, TResponse>`.
- Query handlers do not call `SaveChanges` or `SaveChangesAsync`.
- EF-backed query handlers use no-tracking projections, currently through `AsNoTracking()`.
- Commands and queries use request/response records rather than returning EF/domain entities from API endpoints.
- The default dispatcher is the generated core dispatcher.
- MediatR is optional and, when selected, generated commands/queries and handlers also implement the compatible MediatR abstractions.
- Event sourcing is not part of CQRS-lite in this template and requires a separate ADR before adoption.

## Current Generated Shape

The core dispatcher and abstractions live under `BuildingBlocks/Cqrs`.

Module slices live under `Modules/<ModuleName>/Features/<SliceName>`. Command handlers may mutate state through the owning module DbContext. Query handlers use read projections and should remain side-effect free.

