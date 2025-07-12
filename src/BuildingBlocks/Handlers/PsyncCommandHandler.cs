using System.Text;
using codecrafters_redis.BuildingBlocks.Commands;

namespace codecrafters_redis.BuildingBlocks.Handlers;

public class PsyncCommandHandler : ICommandHandler<Command>
{
    public string HandlingCommandName => Constants.PsyncCommand;
    
    public Task<CommandResult> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        return Task.FromResult<CommandResult>(StreamResult.Create(GetStream()));
    }
    
    private IEnumerable<CommandResult> GetStream()
    {
        yield return SimpleStringResult.Create("FULLRESYNC 8371b4fb1155b71f4a04d3e1bc3e18c4a990aeeb 0");
        
        var fileToReplicate =
            Convert.FromBase64String(
                "UkVESVMwMDEx+glyZWRpcy12ZXIFNy4yLjD6CnJlZGlzLWJpdHPAQPoFY3RpbWXCbQi8ZfoIdXNlZC1tZW3CsMQQAPoIYW9mLWJhc2XAAP/wbjv+wP9aog==");
        string header = $"${fileToReplicate.Length}\r\n";
        byte[] headerBytes = Encoding.UTF8.GetBytes(header);
    
        // Combine header and binary data
        byte[] transmission = new byte[headerBytes.Length + fileToReplicate.Length];
        Buffer.BlockCopy(headerBytes, 0, transmission, 0, headerBytes.Length);
        Buffer.BlockCopy(fileToReplicate, 0, transmission, headerBytes.Length, fileToReplicate.Length);
        
        yield return TransmissionResult.Create(transmission);
    }
}