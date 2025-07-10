using System.Buffers.Binary;
using System.Text;

namespace codecrafters_redis.BuildingBlocks.DB;

public static class DbParser
{
    public static async Task<List<Db>> Parse(Stream file)
    {
        var dbs = new List<Db>();
        
        var magicHeader = await ParseMagicHeader(file);
        var metadata = await ParseMetadata(file);

        Db db = null;
        
        var opCode = (byte)file.ReadByte();
        while (opCode != (byte)RDBOperationCodes.EOF)
        {
            if (opCode == (byte)RDBOperationCodes.DbSelector)
            {
                db = new Db
                {
                    MagicHeader = magicHeader,
                    Metadata = metadata,
                };
                
                dbs.Add(db);

                db.DbNumber = file.ReadByte();
                var sizes = ParseKeyCount(file); // Do we need it? 
            }
            else
            {
                file.Position--;
                
                var (keyValue, keyExpiration) = await ParseData(file);
                db!.KeyValues.Add(keyValue.Key, keyValue.Value);

                if (keyExpiration.HasValue)
                {
                    db.KeyExpirationTimestamps.Add(keyExpiration.Value.Key, keyExpiration.Value.Value);
                }
            }
            
            opCode = (byte)file.ReadByte();
        }
        
        
        return dbs;
    }

    private static async Task<(KeyValuePair<string, object>, KeyValuePair<string, DateTimeOffset>?)> ParseData(Stream stream)
    {
        KeyValuePair<string, DateTimeOffset>? expirationTimes = null;
        var opcode = (byte)stream.ReadByte();
        
        DateTimeOffset? expirationTime = null;
        
        if ((RDBOperationCodes)opcode == RDBOperationCodes.ExpSec)
        {
            var secondsInBytes = new byte[4];
            await stream.ReadExactlyAsync(secondsInBytes);
            var seconds = BinaryPrimitives.ReadInt32LittleEndian(secondsInBytes);
            expirationTime = DateTime.UnixEpoch.AddSeconds(seconds);
        }
        else if ((RDBOperationCodes)opcode == RDBOperationCodes.ExpMilSec)
        {
            var milsecInBytes = new byte[8];
            await stream.ReadExactlyAsync(milsecInBytes);
            var milli = BinaryPrimitives.ReadInt64LittleEndian(milsecInBytes);
            expirationTime = DateTime.UnixEpoch.AddMilliseconds(milli);
        }
        else
        {
            stream.Position--;
        }
        
        var data = await ExtractKeyValue(stream);

        if (expirationTime.HasValue)
        {
            expirationTimes = new KeyValuePair<string, DateTimeOffset>(data.Key, expirationTime.Value);
        }
        
        return (data, expirationTimes);
    }

    private static async Task<KeyValuePair<string, object>> ExtractKeyValue(Stream stream)
    {
        var opcode = (byte)stream.ReadByte();
        if (opcode == (byte)RDBTypes.StringEncoding)
        {
            var key = await ReadKey(stream);
            var valueLength = await GetLength(stream);
            var valueBytes = new byte[valueLength];
            await stream.ReadExactlyAsync(valueBytes);

            return new KeyValuePair<string, object>(key, Encoding.UTF8.GetString(valueBytes));
        }
        else
        {
            throw new NotSupportedException($"Code {opcode} is not supported yet.");
        }
    }

    private static (int keyValueCount, int keyExpirationTimeCount) ParseKeyCount(Stream stream)
    {
        var opcode = stream.ReadByte();
        if (opcode == (byte)RDBOperationCodes.Resizedb)
        {
            return (stream.ReadByte(), stream.ReadByte());
        }
        
        stream.Position--;
        return (0, 0);
    }
    
    private static int ParseDbSelector(Stream stream)
    {
        var dbSelector = stream.ReadByte();
        if(dbSelector == (byte)RDBOperationCodes.DbSelector)
        {
            return stream.ReadByte();
        }

        stream.Position--;
        return 0;
    }

    private static async Task<Dictionary<string, string>> ParseMetadata(Stream stream)
    {
        var metadata = new Dictionary<string, string>();
        var isAuxiliary = stream.ReadByte() == (byte)RDBOperationCodes.Auxiliary;
        while (isAuxiliary)
        {
            var key = await ReadKey(stream);
            var value = await ReadStringValue(stream);

            metadata.Add(key, value);
            isAuxiliary = stream.ReadByte() == (byte)RDBOperationCodes.Auxiliary;
        }

        stream.Position--; // we encounter next DbSelector

        return metadata;
    }

    private static async Task<string> ReadKey(Stream stream)
    {
        var keyLength = await GetLength(stream);
        var keyBytes = new byte[keyLength];
        _ = await stream.ReadAsync(keyBytes);
        var key = Encoding.UTF8.GetString(keyBytes);
        return key;
    }

    private static async Task<string> ReadStringValue(Stream stream)
    {
        var valueStartPosition = stream.Position;
        var valueEndPosition = 0;
        while (!Enum.IsDefined(typeof(RDBOperationCodes), (byte)stream.ReadByte()))
        {
            valueEndPosition++;
        }
            
        var valueLength = valueEndPosition;
        var valueBytes = new byte[valueLength];
            
        stream.Position = valueStartPosition;
        _ = await stream.ReadAsync(valueBytes);
        var value = Encoding.UTF8.GetString(valueBytes);
        return value;
    }

    private static async ValueTask<int> GetLength(Stream stream)
    {
        byte firstByte = (byte)stream.ReadByte();
        var firstTwoBits = firstByte >> 6;

        if (firstTwoBits == 0b00)
        {
            return firstByte;
        }

        if (firstTwoBits == 0b01)
        {
            byte secondByte = (byte)stream.ReadByte();
            return ((firstByte << 2) >> 2) | secondByte;
        }

        if (firstTwoBits == 0b10)
        {
            var bodyLengthBytes = new byte[4];
            _ = await stream.ReadAsync(bodyLengthBytes);
            return BinaryPrimitives.ReadInt32LittleEndian(bodyLengthBytes);
        }

        return 0;
    }
    
    private static async Task<string> ParseMagicHeader(Stream stream)
    {
        const int magicHeaderValue = 5;
        const int magicHeaderVersion = 4;

        var headerValueBites = new byte[magicHeaderValue];
        _ = await stream.ReadAsync(headerValueBites);
        
        var headerVersionBites = new byte[magicHeaderVersion];
        _ = await stream.ReadAsync(headerVersionBites);

        return $"{Encoding.UTF8.GetString(headerValueBites)}{Encoding.UTF8.GetString(headerVersionBites)}";
    }
}