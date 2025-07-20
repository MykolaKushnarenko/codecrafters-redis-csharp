namespace DotRedis.BuildingBlocks.CommandResults;

/// <summary>
///     Represents the result of a transmission operation, containing a binary message payload.
/// </summary>
/// <remarks>
///     Used during replication when transfering the database from master to slaves.
/// </remarks>
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