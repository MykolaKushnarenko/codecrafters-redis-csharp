using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Parsers;

namespace DotRedis.BuildingBlocks;

public interface IMediator
{
    public Task<CommandResult> ProcessAsync(RaspProtocolData context, CancellationToken cancellationToken);
}