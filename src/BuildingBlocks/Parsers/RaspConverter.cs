using System.Text;
using codecrafters_redis.BuildingBlocks;
using DotRedis.BuildingBlocks.CommandResults;

namespace DotRedis.BuildingBlocks.Parsers;

/// <summary>
/// Provides methods for converting <see cref="CommandResult"/> objects into a representation
/// compliant with the RESP (REdis Serialization Protocol) format.
/// </summary>
public static class RaspConverter
{
    public static IEnumerable<byte[]> Convert(CommandResult result)
    {
        return result switch
        {
            SimpleStringResult simpleString => [Encoding.UTF8.GetBytes($"+{simpleString.Message}{Constants.EOL}")],
            ErrorResult error => [Encoding.UTF8.GetBytes($"-ERR {error.ErrorMessage}{Constants.EOL}")],
            IntegerResult integer => [Encoding.UTF8.GetBytes($":{integer.Value}{Constants.EOL}")],
            BulkStringResult bulkString => [SerializeBulkString(bulkString.Value)],
            ArrayResult array => [SerializeArray(array.Items)],
            StreamResult stream => SerializeStream(stream.Stream),
            TransmissionResult transmission => [transmission.Message],
            BulkStringEmptyResult empty => [Encoding.UTF8.GetBytes($"{empty.Value}{Constants.EOL}")],
            ArrayEmptyResult empty => [Encoding.UTF8.GetBytes($"{empty.Value}{Constants.EOL}")],
            MasterReplicationResult => [Enumerable.Empty<byte>().ToArray()],
            _ => throw new InvalidOperationException($"Unknown CommandResponse type: {result.Type}")
        };
    }

    private static IEnumerable<byte[]> SerializeStream(IEnumerable<CommandResult> stream)
    {
        foreach (var item in stream)
        {
            yield return Convert(item).First();
        }
    }

    private static byte[] SerializeBulkString(string[] values)
    {
        if (values.Length == 0)
            return Encoding.UTF8.GetBytes($"$-1{Constants.EOL}");

        var sb = new StringBuilder();
        var length = values.Sum(x => x.Length);
        sb.Append($"${length}{Constants.EOL}");
        
        foreach (var value in values)
        {
            sb.Append(value);
            sb.Append(Constants.EOL);
        }
        
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static byte[] SerializeArray(IEnumerable<CommandResult> items)
    {
        var serializedItems = items.Where(x => x.Type != CommandResultType.MasterReplication).Select(x=> Convert(x).First());
        var flattenedArray = serializedItems.SelectMany(bytes => bytes).ToArray();

        return Encoding.UTF8.GetBytes($"*{items.Count()}{Constants.EOL}")
            .Concat(flattenedArray)
            .ToArray();
    }

}