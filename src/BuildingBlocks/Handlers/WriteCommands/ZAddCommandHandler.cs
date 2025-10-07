using System.Globalization;
using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Storage;

namespace DotRedis.BuildingBlocks.Handlers.WriteCommands;

public class ZAddCommandHandler : ICommandHandler<Command>
{
    private readonly RedisStorage _redisStorage;

    public ZAddCommandHandler(RedisStorage redisStorage)
    {
        _redisStorage = redisStorage;
    }

    public string HandlingCommandName => Constants.ZAddCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var key = command.Arguments[0].ToString();
        var weight = double.Parse(command.Arguments[1].ToString(), CultureInfo.InvariantCulture);
        var value = command.Arguments[2].ToString();
        
        var added = _redisStorage.ZAdd(key, weight, value);

        return Task.FromResult<CommandResult>(IntegerResult.Create(added));
    }
}