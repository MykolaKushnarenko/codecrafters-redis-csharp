namespace codecrafters_redis.BuildingBlocks.DB;

internal enum RdbOperationCodes : byte
{
    Auxiliary = 0xFA,
    DbSelector  = 0xFE,
    Resizedb = 0xFB,
    ExpSec = 0xFD,
    ExpMilSec = 0xFC,
    EOF = 0xFF,
}