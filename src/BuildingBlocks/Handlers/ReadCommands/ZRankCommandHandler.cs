using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Storage;

namespace DotRedis.BuildingBlocks.Handlers.ReadCommands;

public class ZRankCommandHandler : ICommandHandler<Command>
{
    private readonly RedisStorage _storage;

    public ZRankCommandHandler(RedisStorage storage)
    {
        _storage = storage;
    }

    public string HandlingCommandName => Constants.ZRank;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var key = command.Arguments[0].ToString();
        var scoreKey = command.Arguments[1].ToString();

        var score = _storage.ZRank(key, scoreKey);

        if (score.HasValue)
        {
            return Task.FromResult<CommandResult>(IntegerResult.Create(score.Value));
        }

        return Task.FromResult<CommandResult>(new BulkStringEmptyResult());
    }
}