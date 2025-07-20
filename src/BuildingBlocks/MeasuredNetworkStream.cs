using System.Net.Sockets;

namespace DotRedis.BuildingBlocks;

/// <summary>
///     Facade for NetworkStream that tracks bytes read/written.
/// </summary>
/// <remarks>
///     We need this class to track bytes read/written in order to calculate the total size of commands processed which is used in replication.
/// </remarks>
public class MeasuredNetworkStream : Stream
{
    private readonly NetworkStream _innerStream;
    private long _bytes;
    private long _bytesWritten;

    public MeasuredNetworkStream(NetworkStream innerStream)
    {
        _innerStream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
    }

    public long ProcessedCommandBytes => _bytes;
    public long TotalBytesWritten => _bytesWritten;

    public override bool CanRead => _innerStream.CanRead;
    public override bool CanSeek => false;
    public override bool CanWrite => _innerStream.CanWrite;
    public override bool CanTimeout => _innerStream.CanTimeout;

    public override int ReadTimeout
    {
        get => _innerStream.ReadTimeout;
        set => _innerStream.ReadTimeout = value;
    }

    public override int WriteTimeout
    {
        get => _innerStream.WriteTimeout;
        set => _innerStream.WriteTimeout = value;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        int bytesRead = _innerStream.Read(buffer, offset, count);
        Interlocked.Add(ref _bytes, bytesRead);
        return bytesRead;
    }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return ReadAsyncInternal(buffer.AsMemory(offset, count), cancellationToken).AsTask();
    }

    public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return ReadAsyncInternal(buffer, cancellationToken);
    }

    private async ValueTask<int> ReadAsyncInternal(Memory<byte> buffer, CancellationToken cancellationToken)
    {
        int bytesRead = await _innerStream.ReadAsync(buffer, cancellationToken);
        Interlocked.Add(ref _bytes, bytesRead);
        return bytesRead;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _innerStream.Write(buffer, offset, count);
        Interlocked.Add(ref _bytesWritten, count);
    }

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return WriteAsyncInternal(buffer.AsMemory(offset, count), cancellationToken).AsTask();
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return WriteAsyncInternal(buffer, cancellationToken);
    }

    private async ValueTask WriteAsyncInternal(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
    {
        await _innerStream.WriteAsync(buffer, cancellationToken);
        Interlocked.Add(ref _bytesWritten, buffer.Length);
    }

    public override void Flush() => _innerStream.Flush();
    public override Task FlushAsync(CancellationToken cancellationToken) => _innerStream.FlushAsync(cancellationToken);

    // NetworkStream-specific members
    public bool DataAvailable => _innerStream.DataAvailable;
    public void Close() => _innerStream.Close();

    public void Reset()
    {
        _bytes = 0;
        _bytesWritten = 0;
    }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _innerStream.Dispose();
        }
        base.Dispose(disposing);
    }

    public override async ValueTask DisposeAsync()
    {
        await _innerStream.DisposeAsync();
        await base.DisposeAsync();
    }

    // Not supported for NetworkStream
    public override long Length => throw new NotSupportedException();
    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }
    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
    public override void SetLength(long value) => throw new NotSupportedException();
}