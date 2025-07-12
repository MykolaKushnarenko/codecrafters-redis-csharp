namespace codecrafters_redis.BuildingBlocks.Commands;

public class TransmissionResult : CommandResult
{
    public override CommandResultType Type => CommandResultType.SimpleString;
    public byte[] Message { get; }

    private TransmissionResult(byte[] message)
    {
        Message = message;
    }

    public static TransmissionResult Create(byte[] message)
    {
        return new TransmissionResult(message);
    }
}