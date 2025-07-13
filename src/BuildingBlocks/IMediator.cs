using codecrafters_redis.BuildingBlocks.Commands;
using codecrafters_redis.BuildingBlocks.Parsers;

namespace codecrafters_redis.BuildingBlocks;

public interface IMediator
{
    public Task<CommandResult> ProcessAsync(RaspProtocolData context, CancellationToken cancellationToken);
}