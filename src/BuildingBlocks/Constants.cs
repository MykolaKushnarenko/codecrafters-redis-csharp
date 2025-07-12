namespace codecrafters_redis.BuildingBlocks;

public static class Constants
{
    public const string EOL = "\r\n";
    public const string OkResponse = "OK";
    public const string PongResponse = "PONG";
    public const string BulkStringEmptyResponse = "$-1\r\n";

    public const string PingCommand = "PING";
    public const string EchoCommand = "ECHO";
    public const string SetCommand = "SET";
    public const string GetCommand = "GET";
    public const string ConfigCommand = "CONFIG";
    public const string KeysCommand = "KEYS";
    public const string InfoCommand = "INFO";
    public const string RepconfCommand = "REPLCONF";
    public const string PsyncCommand = "PSYNC";
    
    public const string DirArgument = "--dir";
    public const string DbFileNameArgument = "--dbfilename";
    public const string PortArgument = "--port";
    public const string ReplicaOfArgument = "--replicaof";
}