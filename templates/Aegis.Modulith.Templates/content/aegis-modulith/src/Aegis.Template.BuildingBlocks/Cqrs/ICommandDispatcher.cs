namespace Aegis.Template.BuildingBlocks.Cqrs;

public interface ICommandDispatcher
{
    Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
}
