namespace Aegis.Template.BuildingBlocks.Cqrs;

public interface IQueryDispatcher
{
    Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
}
