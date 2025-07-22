using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Storage;

namespace DotRedis.BuildingBlocks.Handlers;

public class XReadCommandHandler : ICommandHandler<Command>
{
    private const int ArgumentKeyDivider = 2; // Extracted constant
    private readonly RedisStorage _storage;

    public XReadCommandHandler(RedisStorage storage)
    {
        _storage = storage;
    }

    public string HandlingCommandName => Constants.XReadCommand;

    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var numberOfStreamKeys = (command.Arguments.Length - 1) / ArgumentKeyDivider;
        var streamKeys = ExtractStreamKeys(command, numberOfStreamKeys);
        var streamKeyIndex = numberOfStreamKeys + 1;

        var result = new List<ArrayResult>();
        foreach (var streamKey in streamKeys)
        {
            var streamResult = ProcessStream(streamKey, command, streamKeyIndex);
            result.Add(streamResult);
            streamKeyIndex++;
        }

        return Task.FromResult<CommandResult>(ArrayResult.Create(result.ToArray()));
    }

    private List<string> ExtractStreamKeys(Command command, int numberOfStreamKeys)
    {
        var streamKeys = new List<string>();
        for (int i = 1; i <= numberOfStreamKeys; i++)
        {
            streamKeys.Add(command.Arguments[i].ToString());
        }
        return streamKeys;
    }

    private ArrayResult ProcessStream(string streamKey, Command command, int streamKeyIndex)
    {
        var streamResult = new List<ArrayResult>();
        var stream = _storage.GetStream(streamKey);

        var streamId = command.Arguments[streamKeyIndex].ToString();
        var entries = stream.Read(streamId);

        foreach (var entry in entries)
        {
            streamResult.Add(ProcessEntry(entry));
        }

        return ArrayResult.Create(BulkStringResult.Create(streamKey), ArrayResult.Create(streamResult.ToArray()));
    }

    private ArrayResult ProcessEntry(StreamEntry entry) // Assuming entry type
    {
        var id = BulkStringResult.Create(entry.Id);
        var fields = new List<BulkStringResult>();

        foreach (var field in entry.Fields)
        {
            fields.Add(BulkStringResult.Create(field.Key));
            fields.Add(BulkStringResult.Create(field.Value.Value.ToString()));
        }

        return ArrayResult.Create(id, ArrayResult.Create(fields.ToArray()));
    }
}