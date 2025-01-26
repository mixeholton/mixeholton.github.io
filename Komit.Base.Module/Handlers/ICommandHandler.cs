using Komit.Base.Module.Handlers.Context.Markers;
namespace Komit.Base.Module.Handlers;
public interface ICommandHandler<TCommand> : IRequestMarker<TCommand>
    where TCommand : CommandBase
{
    public Task Perform(TCommand command);
}