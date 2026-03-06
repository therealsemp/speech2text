namespace speech2text.Domain.Ports;

/// <summary>
/// Captures audio from the microphone for the duration of a recording session.
/// Recording starts when the method is called and stops when the cancellation token is triggered.
/// Returns the captured audio as a WAV byte array ready to be sent to a transcription backend.
/// </summary>
public interface IAudioCapture
{
    Task<byte[]> RecordAsync(CancellationToken ct);
}
