namespace codecrafters_redis.BuildingBlocks;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    public string HandlingCommandName { get; }
    
    Task<byte[]> HandleAsync(TCommand command);
}