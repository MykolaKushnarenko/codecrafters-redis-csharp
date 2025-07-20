using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;

namespace DotRedis.BuildingBlocks.Handlers;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    public string HandlingCommandName { get; }
    
    Task<CommandResult> HandleAsync(TCommand command, CancellationToken cancellationToken);
}