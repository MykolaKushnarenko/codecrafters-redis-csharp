namespace DotRedis.BuildingBlocks;

public static class Constants
{
    public const string EOL = "\r\n";
    public const string OkResponse = "OK";
    public const string PongResponse = "PONG";

    public const string PingCommand = "PING";
    public const string EchoCommand = "ECHO";
    public const string SetCommand = "SET";
    public const string GetCommand = "GET";
    public const string ConfigCommand = "CONFIG";
    public const string KeysCommand = "KEYS";
    public const string InfoCommand = "INFO";
    public const string RepconfCommand = "REPLCONF";
    public const string PsyncCommand = "PSYNC";
    public const string WaitCommand = "WAIT";
    public const string TypeCommand = "TYPE";
    public const string XAddCommand = "XADD";
    public const string XRangeCommand = "XRANGE";
    public const string XReadCommand = "XREAD";
    public const string IncrCommand = "INCR";
    public const string MultiCommand = "MULTI";
    public const string ExecCommand = "EXEC";
    public const string DiscardCommand = "DISCARD";
    public const string RPushCommand = "RPUSH";
    public const string LPushCommand = "LPUSH";
    public const string LRangeCommand = "LRANGE";
    public const string LLenCommand = "LLEN";
    public const string LPopCommand = "LPOP";
    public const string BLPopCommand = "BLPOP";
    public const string SubscribeCommand = "SUBSCRIBE";
    public const string PublishCommand = "PUBLISH";
    public const string UnsubscribeCommand = "UNSUBSCRIBE";
    public const string ZAddCommand = "ZADD";
    
    public const string DirArgument = "--dir";
    public const string DbFileNameArgument = "--dbfilename";
    public const string PortArgument = "--port";
    public const string ReplicaOfArgument = "--replicaof";
}