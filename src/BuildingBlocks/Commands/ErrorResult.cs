namespace codecrafters_redis.BuildingBlocks.Commands;

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