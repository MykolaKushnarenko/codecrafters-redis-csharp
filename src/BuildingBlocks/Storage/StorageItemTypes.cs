namespace codecrafters_redis.BuildingBlocks.Storage;

public enum StorageItemTypes
{
    String,
    Hash,
    List,
    Set,
    SortedSet,
    VectorSet,
    Stream,
    Bitmap,
    Bitfield,
    Geospatial,
    JSON,
    ProbabilisticDataTypes,
    TimeSeries,
}

public static class StorageItemTypeMapper
{
    private static readonly Dictionary<Type, StorageItemTypes> TypeMappings = new()
    {
        { typeof(string), StorageItemTypes.String },
        { typeof(Dictionary<,>), StorageItemTypes.Hash },
        { typeof(List<>), StorageItemTypes.List },
        { typeof(HashSet<string>), StorageItemTypes.Set },
        { typeof(RStream) , StorageItemTypes.Stream  }
    };

    public static StorageItemTypes GetStorageType(Type type)
    {
        return TypeMappings.TryGetValue(type, out var storageType) 
            ? storageType 
            : throw new ArgumentException($"No mapping defined for type {type.Name}");
    }
}