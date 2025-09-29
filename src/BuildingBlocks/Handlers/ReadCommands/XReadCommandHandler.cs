using DotRedis.BuildingBlocks.CommandResults;
using DotRedis.BuildingBlocks.Commands;
using DotRedis.BuildingBlocks.Services;
using DotRedis.BuildingBlocks.Storage;

namespace DotRedis.BuildingBlocks.Handlers.ReadCommands;

public class XReadCommandHandler : ICommandHandler<Command>
{
    private const int ArgumentKeyDivider = 2;
    private readonly RedisStorage _storage;
    private readonly RedisValueListener _listener;

    private const int SkipBlockWaitStreamsPosition = 3;
    private const int SkipStreamsPosition = 1;
    
    public XReadCommandHandler(RedisStorage storage, RedisValueListener listener)
    {
        _storage = storage;
        _listener = listener;
    }

    public string HandlingCommandName => Constants.XReadCommand;

    public async Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var shouldWaitForLatestItems = string.Equals(command.Arguments[^1].ToString(), "$", StringComparison.CurrentCultureIgnoreCase);
        var isBlocking = string.Equals(command.Arguments[0].ToString(), "BLOCK", StringComparison.CurrentCultureIgnoreCase);

        var streamKeysWithIds = new List<(string streamKey, string streamId)>();
        
        if (isBlocking)
        {
            if (shouldWaitForLatestItems)
            {
                command.Arguments = command.Arguments[..^1];
                streamKeysWithIds = ExtractLatestIdsFromStreams(command.Arguments, SkipBlockWaitStreamsPosition);
            }
            else
            {
                streamKeysWithIds = ExtractStreamKeysWithIds(command.Arguments, SkipBlockWaitStreamsPosition); 
            }
            
            var waitTime = int.Parse(command.Arguments[1].ToString());

            if (waitTime == 0)
            {
                await _listener.WaitForNewDataAsync(streamKeysWithIds[0].streamKey);
            }
            else
            {
                await Task.Delay(TimeSpan.FromMilliseconds(waitTime), cancellationToken);
            }
        }
        else
        {
            streamKeysWithIds = ExtractStreamKeysWithIds(command.Arguments, SkipStreamsPosition); 
        }
        
        var streamResults = streamKeysWithIds
            .Select(streamKey => ProcessStream(streamKey.streamKey, streamKey.streamId))
            .Where(result => result is not BulkStringEmptyResult)
            .ToArray();

        if (streamResults.Length != 0)
        {
            return ArrayResult.Create(streamResults);
        }
        
        return new ArrayEmptyResult();
    }

    private List<(string streamKey, string streamId)> ExtractStreamKeysWithIds(object[] arguments, int startPosition)
    {
        var count = (arguments.Length - startPosition) / ArgumentKeyDivider;
        var streamKeys = new List<(string, string)>();
        while (count > 0)
        {
            var key = arguments[startPosition].ToString();
            var id = arguments[^count].ToString();
            
            count--;
            startPosition++;
            streamKeys.Add((key, id));
        }
        
        return streamKeys;
    }
    
    private List<(string streamKey, string streamId)> ExtractLatestIdsFromStreams(object[] arguments, int startPosition)
    {
        var streamKeys = new List<(string, string)>();
        
        var steamKeysCounter = arguments.Length - startPosition;
        var cursor = startPosition;
        
        Console.WriteLine($"Processing stream");
        while (steamKeysCounter > 0)
        {
            var streamKey = arguments[cursor].ToString();
            
            Console.WriteLine($"Processing stream {streamKey}");
            
            var stream = _storage.GetStream(streamKey);
            var lastStreamEntry = stream.ReadLatest();
            
            streamKeys.Add((streamKey, lastStreamEntry.Id));

            steamKeysCounter--;
            cursor++;
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

    private ArrayResult ProcessEntry(StreamEntry entry)
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