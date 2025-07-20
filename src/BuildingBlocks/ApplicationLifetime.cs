namespace DotRedis.BuildingBlocks;

/// <summary>
///     Manages the application lifetime by providing mechanisms to handle
///     application shutdown and disposal activities.
/// </summary>
public class ApplicationLifetime : IDisposable
{
    private readonly CancellationTokenSource _cts = new();
    private ConsoleCancelEventHandler _shutdownHandler;
    
    public CancellationToken CreateApplicationCancellationToken()
    {
        _shutdownHandler = (sender, e) => 
        {
            e.Cancel = true; 
            _cts.Cancel();
        };
        
        Console.CancelKeyPress += _shutdownHandler;
        
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