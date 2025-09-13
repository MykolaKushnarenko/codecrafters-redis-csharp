using System.Text;

namespace DotRedis.BuildingBlocks.Parsers;

/// <summary>
/// A utility class for parsing RASP (Redis Advanced Serialization Protocol) commands and bulk binary data from input streams.
/// </summary>
public static class RaspProtocolParser
{
    public static async Task<byte[]> ParseBinaryAsync(Stream input)
    {
        byte firstByte = (byte)input.ReadByte();
        if (firstByte != RaspTypes.BulkStrings)
        {
            throw new InvalidOperationException("Invalid binary protocol header");
        }
        
        var length = int.Parse(ReadLine(input));
        var bulkString = new byte[length];
        await input.ReadExactlyAsync(bulkString, 0, length);
        
        return bulkString;
    }
    
    public static async Task<RaspProtocolData> ParseCommand(Stream input, CancellationToken cancellationToken)
    {
        var result = await ParseProtocol(input, cancellationToken);

        if (result is null)
        {
            throw new InvalidOperationException("Invalid command.");
        }
        
        return result switch
        {
            string command => new RaspProtocolData { Name = command },
            object[] array => new RaspProtocolData { Name = array[0].ToString()!, Arguments = array[1..] },
            _ => null
        };
    }
    
    private static async Task<object> ParseProtocol(Stream input, CancellationToken cancellationToken)
    {
        var buffer = new byte[1];
        await input.ReadExactlyAsync(buffer, cancellationToken);
        switch (buffer[0])
        {
            case RaspTypes.SimpleStrings:
                return ReadLine(input);
            case RaspTypes.Integers:
            {
                if (int.TryParse(ReadLine(input), out var result))
                {
                    return result;
                }

                break;
            }
            case RaspTypes.BulkStrings:
            {
                var result = ReadLine(input);
                var length = int.Parse(result);
                var bulkString = new byte[length];
                await input.ReadExactlyAsync(bulkString, 0, length, cancellationToken);

                await input.ReadExactlyAsync(new byte[2], cancellationToken);

                return Encoding.UTF8.GetString(bulkString);
            }
            case RaspTypes.Arrays:
            {
                var arrayLengthsString = ReadLine(input);

                if (!int.TryParse(arrayLengthsString, out var arrayLengths))
                {
                    return null;
                }
                
                var array = new object[arrayLengths];

                for (int i = 0; i < arrayLengths; i++)
                {
                    var arrayItem = await ParseProtocol(input, cancellationToken);
                    array[i] = arrayItem;
                }

                return array;
            }
        }

        throw new InvalidOperationException($"Unknown protocol type: {buffer[0]}");
    }
    
    private static string ReadLine(Stream stream)
    {
        var builder = new StringBuilder();
        
        int currentByte;
        int previousByte = 0;
        while ((currentByte = stream.ReadByte()) != -1)
        {
            if (previousByte == '\r' && currentByte == '\n')
            {
                break;
            }

            if (currentByte is '\r')
            {
                previousByte = currentByte;
                continue;
            }
            
            builder.Append((char)currentByte);
            previousByte = currentByte;
        }
        
        return builder.ToString();
    }
}