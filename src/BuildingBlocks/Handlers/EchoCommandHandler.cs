using codecrafters_redis.BuildingBlocks;
using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;

namespace DotRedis.BuildingBlocks.Handlers;

/// <summary>
///     Handles the "ECHO" command in a Redis-like system.
/// </summary>
/// <remarks>
///     Redis link: https://redis.io/docs/latest/commands/echo/
/// </remarks>
/// <example>
///     This handler processes a command by returning a <see cref="BulkStringResult"/> that contains the echoed argument
///     obtained from the command's input.
/// </example>
public class EchoCommandHandler : ICommandHandler<Command>
{
    public string HandlingCommandName => Constants.EchoCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        return Task.FromResult<CommandResult>(BulkStringResult.Create(command.Arguments[0].ToString()));
    }
}