namespace codecrafters_redis.BuildingBlocks;

public static class Constants
{
    public const string EOL = "\r\n";
    public const string OkResponse = "+OK\r\n";
    public const string PongResponse = "+PONG\r\n";
    public const string BulkStringEmptyResponse = "$-1\r\n";
}