namespace codecrafters_redis.BuildingBlocks;

public class ApplicationLifetime : IDisposable
{
    private readonly CancellationTokenSource _cts = new();
    private ConsoleCancelEventHandler _shutdownHandler;
    
    public CancellationToken CreateApplicationCancellationToken()
    {
        _shutdownHandler = (sender, e) => 
        {
            e.Cancel = true; // Prevent immediate termination
            _cts.Cancel();  // Trigger graceful shutdown
        };
        
        Console.CancelKeyPress += _shutdownHandler;
        
        // Also listens to ProcessExit via AppDomain
        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

        return _cts.Token;
    }
    
    private void OnProcessExit(object sender, EventArgs e)
    {
        Console.WriteLine("Process exit");
        _cts.Cancel();
    }

    public void Dispose()
    {
        _cts.Dispose();
    }
}