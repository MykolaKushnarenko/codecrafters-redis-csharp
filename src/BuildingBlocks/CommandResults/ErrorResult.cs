namespace DotRedis.BuildingBlocks.CommandResults;

/// <summary>
///     Represents a command result of type Error.
/// </summary>
/// <remarks>
///     Redis link: https://redis.io/docs/latest/develop/reference/protocol-spec/#bulk-errors
/// </remarks>
public class ErrorResult : CommandResult
{
    public override CommandResultType Type => CommandResultType.Error;
    public string ErrorMessage { get; }

    private ErrorResult(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }
    
    public static ErrorResult Create(string message)
    {
        return new ErrorResult(message);
    }
}