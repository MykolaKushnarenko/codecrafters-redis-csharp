using System.Buffers.Binary;
using System.Text;

namespace codecrafters_redis.BuildingBlocks.DB;

public static class RdbParser
{
    public static async Task<List<RDbSnapshot>> ParseAsync(Stream file, CancellationToken cancellationToken)
    {
        var dbs = new List<RDbSnapshot>();
        
        var magicHeader = await ParseMagicHeaderAsync(file, cancellationToken);
        var metadata = await ParseMetadataAsync(file, cancellationToken);

        RDbSnapshot rDbSnapshot = null;
        
        var opCode = (byte)file.ReadByte();
        while (opCode != (byte)RdbOperationCodes.EOF)
        {
            if (opCode == (byte)RdbOperationCodes.DbSelector)
            {
                rDbSnapshot = new RDbSnapshot
                {
                    MagicHeader = magicHeader,
                    Metadata = metadata,
                };
                
                dbs.Add(rDbSnapshot);

                rDbSnapshot.DbNumber = file.ReadByte();
                var sizes = ParseKeyCount(file); // Do we need it? 
            }
            else
            {
                file.Position--;
                
                var (keyValue, keyExpiration) = await ParseDataAsync(file, cancellationToken);
                rDbSnapshot!.KeyValues.Add(keyValue.Key, keyValue.Value);

                if (keyExpiration.HasValue)
                {
                    rDbSnapshot.KeyExpirationTimestamps.Add(keyExpiration.Value.Key, keyExpiration.Value.Value);
                }
            }
            
            opCode = (byte)file.ReadByte();
        }
        
        
        return dbs;
    }

    private static async Task<(KeyValuePair<string, object>, KeyValuePair<string, DateTimeOffset>?)> ParseDataAsync(Stream stream, CancellationToken cancellationToken)
    {
        KeyValuePair<string, DateTimeOffset>? expirationTimes = null;
        var opcode = (byte)stream.ReadByte();
        
        DateTimeOffset? expirationTime = null;
        
        if ((RdbOperationCodes)opcode == RdbOperationCodes.ExpSec)
        {
            var secondsInBytes = new byte[4];
            await stream.ReadExactlyAsync(secondsInBytes, cancellationToken);
            var seconds = BinaryPrimitives.ReadInt32LittleEndian(secondsInBytes);
            expirationTime = DateTime.UnixEpoch.AddSeconds(seconds);
        }
        else if ((RdbOperationCodes)opcode == RdbOperationCodes.ExpMilSec)
        {
            var milsecInBytes = new byte[8];
            await stream.ReadExactlyAsync(milsecInBytes, cancellationToken);
            var milli = BinaryPrimitives.ReadInt64LittleEndian(milsecInBytes);
            expirationTime = DateTime.UnixEpoch.AddMilliseconds(milli);
        }
        else
        {
            stream.Position--;
        }
        
        var data = await ExtractKeyValue(stream, cancellationToken);

        if (expirationTime.HasValue)
        {
            expirationTimes = new KeyValuePair<string, DateTimeOffset>(data.Key, expirationTime.Value);
        }
        
        return (data, expirationTimes);
    }

    private static async Task<KeyValuePair<string, object>> ExtractKeyValue(Stream stream, CancellationToken cancellationToken)
    {
        var opcode = (byte)stream.ReadByte();
        if (opcode == (byte)RdbTypes.StringEncoding)
        {
            var key = await ReadKeyAsync(stream, cancellationToken);
            var valueLength = await GetLengthAsync(stream, cancellationToken);
            var valueBytes = new byte[valueLength];
            await stream.ReadExactlyAsync(valueBytes, cancellationToken);

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
        if (opcode == (byte)RdbOperationCodes.Resizedb)
        {
            return (stream.ReadByte(), stream.ReadByte());
        }
        
        stream.Position--;
        return (0, 0);
    }

    private static async Task<Dictionary<string, string>> ParseMetadataAsync(Stream stream, CancellationToken cancellationToken)
    {
        var metadata = new Dictionary<string, string>();
        var isAuxiliary = stream.ReadByte() == (byte)RdbOperationCodes.Auxiliary;
        while (isAuxiliary)
        {
            var key = await ReadKeyAsync(stream, cancellationToken);
            var value = await ReadStringValueAsync(stream, cancellationToken);

            metadata.Add(key, value);
            isAuxiliary = stream.ReadByte() == (byte)RdbOperationCodes.Auxiliary;
        }

        stream.Position--; // we encounter next DbSelector

        return metadata;
    }

    private static async Task<string> ReadKeyAsync(Stream stream, CancellationToken cancellationToken)
    {
        var keyLength = await GetLengthAsync(stream, cancellationToken);
        var keyBytes = new byte[keyLength];
        _ = await stream.ReadAsync(keyBytes, cancellationToken);
        var key = Encoding.UTF8.GetString(keyBytes);
        return key;
    }

    private static async Task<string> ReadStringValueAsync(Stream stream, CancellationToken cancellationToken)
    {
        var valueStartPosition = stream.Position;
        var valueEndPosition = 0;
        while (!Enum.IsDefined(typeof(RdbOperationCodes), (byte)stream.ReadByte()))
        {
            valueEndPosition++;
        }
            
        var valueLength = valueEndPosition;
        var valueBytes = new byte[valueLength];
            
        stream.Position = valueStartPosition;
        _ = await stream.ReadAsync(valueBytes, cancellationToken);
        var value = Encoding.UTF8.GetString(valueBytes);
        return value;
    }

    private static async ValueTask<int> GetLengthAsync(Stream stream, CancellationToken cancellationToken)
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
            _ = await stream.ReadAsync(bodyLengthBytes, cancellationToken);
            return BinaryPrimitives.ReadInt32LittleEndian(bodyLengthBytes);
        }

        return 0;
    }
    
    private static async Task<string> ParseMagicHeaderAsync(Stream stream, CancellationToken cancellationToken)
    {
        const int magicHeaderValue = 5;
        const int magicHeaderVersion = 4;

        var headerValueBites = new byte[magicHeaderValue];
        _ = await stream.ReadAsync(headerValueBites, cancellationToken);
        
        var headerVersionBites = new byte[magicHeaderVersion];
        _ = await stream.ReadAsync(headerVersionBites, cancellationToken);

        return $"{Encoding.UTF8.GetString(headerValueBites)}{Encoding.UTF8.GetString(headerVersionBites)}";
    }
}