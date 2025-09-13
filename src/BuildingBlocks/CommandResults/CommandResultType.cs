namespace DotRedis.BuildingBlocks.CommandResults;

public enum CommandResultType
{
    SimpleString,           // E.g., "+OK"
    Error,                  // E.g., "-ERR something went wrong"
    Integer,                // E.g., ":100"
    BulkString,             // E.g., "$6\r\nfoobar"
    Array,                  // E.g., "*2\r\n$3\r\nfoo\r\n$3\r\nbar"
    Stream,                 // For streaming incremental responses
    MasterReplication,      // For MASTER REPLICATION responses (this is a marker for no response).
    NullArray,              // For null array responses
}