using System.Text;

namespace codecrafters_redis.BuildingBlocks.Parsers;

public static class ProtocolParser
{
    public static ProtocolParseResult? Parse(Stream input)
    {
        var result = ParseProtocol(input);

        return result switch
        {
            string command => new ProtocolParseResult { Name = command },
            object[] array => new ProtocolParseResult { Name = array[0].ToString()!, Arguments = array[1..] },
            _ => null
        };
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