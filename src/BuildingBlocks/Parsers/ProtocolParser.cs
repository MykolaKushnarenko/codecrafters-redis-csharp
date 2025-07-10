using System.Net.Sockets;
using System.Text;
using codecrafters_redis.BuildingBlocks.Parsers;

namespace codecrafters_redis;

public static class ProtocolParser
{
    // *2\r\n$4\r\nECHO\r\n$3\r\nhey\r\n

    public static ProtocolParseResult? Parse(Stream input)
    {
        var result = ParseProtocol(input);

        switch (result)
        {
            case string command:
                return new ProtocolParseResult
                {
                    Name = command
                };
            case object[] array:
                return new ProtocolParseResult
                {
                    Name = array[0].ToString(),
                    Arguments = array[1..]
                };
            default: return null;
        }
    }
    
    public static object ParseProtocol(Stream input)
    {
        byte firstByte = (byte)input.ReadByte();
        switch (firstByte)
        {
            case RedisType.SimpleStrings:
                return ReadLine(input);
            case RedisType.Integers:
            {
                if (int.TryParse(ReadLine(input), out var result))
                {
                    return result;
                }

                break;
            }
            case RedisType.BulkStrings:
            {
                var result = ReadLine(input);
                var length = int.Parse(result);
                var bulkString = new byte[length];
                input.ReadExactly(bulkString, 0, length);
                
                input.Position+=2; // skip \r\n

                return Encoding.UTF8.GetString(bulkString);
            }
            case RedisType.Arrays:
            {
                var arrayLengthsString = ReadLine(input);

                if (!int.TryParse(arrayLengthsString, out var arrayLengths))
                {
                    return null;
                }
                
                var array = new object[arrayLengths];

                for (int i = 0; i < arrayLengths; i++)
                {
                    var arrayItem = ParseProtocol(input);
                    array[i] = arrayItem;
                }

                return array;
            }
        }

        return null;
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