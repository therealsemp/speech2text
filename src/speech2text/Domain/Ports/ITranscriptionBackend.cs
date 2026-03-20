namespace speech2text.Domain.Ports;

/// <summary>
/// Sends audio data to a speech-to-text service and returns the transcribed text.
/// Each implementation targets a specific cloud or local transcription provider.
/// The domain is agnostic of the underlying technology — new backends can be added
/// without touching the domain or application layers.
/// </summary>
public interface ITranscriptionBackend
{
    Task<string> TranscribeAsync(byte[] audioData, string language, CancellationToken ct);
}
