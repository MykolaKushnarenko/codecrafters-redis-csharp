namespace DotRedis.BuildingBlocks.CommandResults;

/// <summary>
///     Represents the base class for all command results in the library.
/// </summary>
/// <remarks>
///     Derived classes provide specific implementations for various result types,
///     such as arrays, bulk strings, and error results. This base class defines
///     the general contract for result types.
/// </remarks>
public abstract class CommandResult
{
    public abstract CommandResultType Type { get; }
}