namespace DotRedis.BuildingBlocks.Rdb;

/// <summary>
///     Represents the different types of data encodings used in a Redis RDB file.
/// </summary>
/// <remarks>
///     Doc link: https://rdb.fnordig.de/file_format.html#value-type
/// </remarks>
internal enum RdbTypes
{
    StringEncoding = 0,
    ListEncoding = 1,
    SetEncoding = 2,
    SortedSetEncoding = 3,
    HashEncoding = 4,
    ZipmapEncoding = 9,
    ZiplistEncoding = 10,
    IntsetEncoding = 11,
    SortedSetinZiplistEncoding = 12,
    HashmapinZiplistEncoding = 13,
    ListinQuicklistEncoding = 14,
}