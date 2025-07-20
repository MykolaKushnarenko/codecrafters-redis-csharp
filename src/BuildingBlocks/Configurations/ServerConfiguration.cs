namespace DotRedis.BuildingBlocks.Configurations;

/// <summary>
///     Represents the configuration settings for a server in the Redis-like system.
/// </summary>
public class ServerConfiguration
{
    /// <summary>
    ///     Represents the directory where the server files, such as database files, may be stored.
    /// </summary>
    public string Dir { get; set; }

    /// <summary>
    ///     Gets or sets the name of the database file used for storing data persistently.
    /// </summary>
    /// <remarks>
    ///     This property is used in conjunction with the "Dir" property to determine the full
    ///     file path for the database file. It is essential for database rehydration and persistence.
    /// </remarks>
    public string DbFileName { get; set; }

    /// <summary>
    ///     Gets or sets the port on which the server listens for incoming connections.
    /// </summary>
    /// <remarks>
    ///     The default value is 6379. This setting determines the port number the server uses to handle client connections
    ///     and should match the configuration of clients for successful communication.
    /// </remarks>
    public int Port { get; set; } = 6379;

    /// <summary>
    ///     Gets or sets the role of the server in the Redis network configuration.
    /// </summary>
    /// <remarks>
    ///     The role determines whether the server operates as a "master" or a "slave". The default value is "master".
    /// </remarks>
    public string Role { get; set; } = "master";

    /// <summary>
    ///     Specifies the host address of the master server in a replication configuration.
    ///     This property is used when the current server is configured as a slave, allowing it
    ///     to connect to the master server for data synchronization.
    /// </summary>
    public string MasterHost { get; set; }

    /// <summary>
    ///     Represents the port number of the master server in a replication setup.
    /// </summary>
    /// <remarks>
    ///     Used when configuring a slave server to connect to its master. This value
    ///     specifies the port on which the master server is listening for incoming connections.
    /// </remarks>
    public int MasterPort { get; set; }
}