namespace DotRedis.BuildingBlocks.Rdb;

/// <summary>
///     Represents the operational codes used in Redis RDB file processing.
/// </summary>
/// <remarks>
///     Doc link: https://rdb.fnordig.de/file_format.html#op-codes
/// </remarks>
internal enum RdbOperationCodes : byte
{
    Auxiliary = 0xFA,
    DbSelector  = 0xFE,
    Resizedb = 0xFB,
    ExpSec = 0xFD,
    ExpMilSec = 0xFC,
    EOF = 0xFF,
}