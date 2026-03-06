namespace speech2text.Domain.Ports;

public interface IAudioCapture
{
    Task<byte[]> RecordAsync(CancellationToken ct);
}
