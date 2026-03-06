using System.IO;
using NAudio.Wave;
using speech2text.Domain.Ports;

namespace speech2text.Adapters.Audio;

/// <summary>
/// Records audio from the default microphone using the NAudio library (WaveInEvent).
/// Captures raw PCM at 16 kHz mono — the format recommended by Whisper for optimal transcription quality.
/// Recording runs until the cancellation token fires, at which point WaveInEvent is stopped gracefully
/// and the captured PCM is wrapped into a WAV file returned as a byte array.
/// </summary>
public class NAudioCaptureAdapter : IAudioCapture
{
    // 16 kHz mono — format optimal pour Whisper
    private static readonly WaveFormat RecordingFormat = new(16000, 1);

    public async Task<byte[]> RecordAsync(CancellationToken ct)
    {
        using var waveIn = new WaveInEvent { WaveFormat = RecordingFormat };

        var rawPcm = new MemoryStream();
        waveIn.DataAvailable += (_, e) => rawPcm.Write(e.Buffer, 0, e.BytesRecorded);

        var stoppedTcs = new TaskCompletionSource();
        waveIn.RecordingStopped += (_, _) => stoppedTcs.TrySetResult();

        ct.Register(() => waveIn.StopRecording());

        waveIn.StartRecording();
        await stoppedTcs.Task;

        return WrapInWav(rawPcm.ToArray(), waveIn.WaveFormat);
    }

    private static byte[] WrapInWav(byte[] rawPcm, WaveFormat format)
    {
        var wavStream = new MemoryStream();
        using (var writer = new WaveFileWriter(wavStream, format))
            writer.Write(rawPcm, 0, rawPcm.Length);
        // MemoryStream.ToArray() reste accessible après Dispose du WaveFileWriter
        return wavStream.ToArray();
    }
}
