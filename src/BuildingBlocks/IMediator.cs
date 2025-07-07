namespace codecrafters_redis.BuildingBlocks;

public interface IMediator
{
    public Task Process(Context context);
}