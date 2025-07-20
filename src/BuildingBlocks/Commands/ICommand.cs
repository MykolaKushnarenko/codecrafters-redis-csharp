namespace DotRedis.BuildingBlocks.Commands;

public interface ICommand
{
    object[] Arguments { get; set; }
}