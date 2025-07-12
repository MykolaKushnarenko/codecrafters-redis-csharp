namespace codecrafters_redis.BuildingBlocks.DB;

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