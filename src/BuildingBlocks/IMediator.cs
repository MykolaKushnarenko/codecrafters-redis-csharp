namespace codecrafters_redis.BuildingBlocks;

public interface IMediator
{
    public Task ProcessAsync(Context context, CancellationToken cancellationToken);
}