namespace codecrafters_redis.BuildingBlocks;

public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task<byte[]> HandleAsync(TCommand command);
}