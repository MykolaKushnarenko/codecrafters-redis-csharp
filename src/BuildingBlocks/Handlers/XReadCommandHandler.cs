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

    public async Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var position = 0;
        
        var isBlocking = string.Equals(command.Arguments[0].ToString(), "BLOCK", StringComparison.CurrentCultureIgnoreCase);
        
        if (isBlocking)
        {
            var waitTime = int.Parse(command.Arguments[1].ToString());
            await Task.Delay(TimeSpan.FromMilliseconds(waitTime), cancellationToken);
            position = 3; // Skip BLOCK, WAIT TIME AND `STREAMS` 
        }
        else
        {
            position = 1; //Skip `STREAMS`
        }
        var streamKeysWithIds = ExtractStreamKeysWithIds(command, position);

        var streamResults = streamKeysWithIds
            .Select(streamKey => ProcessStream(streamKey.streamKey, streamKey.streamId))
            .Where(result => result is not BulkStringEmptyResult)
            .ToArray();

        if (streamResults.Length != 0)
        {
            return ArrayResult.Create(streamResults);
        }
        
        return new BulkStringEmptyResult();
    }

    private List<(string streamKey, string streamId)> ExtractStreamKeysWithIds(Command command, int startPosition)
    {
        var count = (command.Arguments.Length - startPosition) / ArgumentKeyDivider;
        
        var streamKeys = new List<(string, string)>();
        while (count > 0)
        {
            var key = command.Arguments[startPosition].ToString();
            var id = command.Arguments[^count].ToString();
            
            count--;
            startPosition++;
            streamKeys.Add((key, id));
        }
        
        return streamKeys;
    }

    private CommandResult ProcessStream(string streamKey, string streamId)
    {
        var streamResult = new List<ArrayResult>();
        var stream = _storage.GetStream(streamKey);

        var entries = stream.Read(streamId);

        if (entries.Length == 0)
        {
            return new BulkStringEmptyResult();
        }
        
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