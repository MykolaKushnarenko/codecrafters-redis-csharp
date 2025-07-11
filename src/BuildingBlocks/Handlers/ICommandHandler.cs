using codecrafters_redis.BuildingBlocks.Commands;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    public string HandlingCommandName { get; }
    
    Task<byte[]> HandleAsync(TCommand command);
}