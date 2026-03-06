namespace speech2text.Domain.Ports;

public interface ITranscriptionBackend
{
    Task<string> TranscribeAsync(byte[] audioData, string language, CancellationToken ct);
}
