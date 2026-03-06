using System.IO;
using Azure;
using Azure.AI.OpenAI;
using OpenAI.Audio;
using speech2text.Domain;
using speech2text.Domain.Ports;

namespace speech2text.Adapters.Transcription;

/// <summary>
/// Transcribes audio using the Azure OpenAI Whisper model via the Azure.AI.OpenAI SDK (v2).
/// Both <see cref="AzureOpenAIClient"/> and <see cref="AudioClient"/> are initialized once at
/// construction time and reused across calls — they are thread-safe and manage an internal
/// HttpClient with connection pooling, so recreating them per call would be wasteful.
/// </summary>
public class AzureOpenAITranscriptionAdapter : ITranscriptionBackend
{
    private readonly AudioClient _audioClient;

    public AzureOpenAITranscriptionAdapter(TranscriptionProfile profile)
    {
        var client = new AzureOpenAIClient(
            new Uri(profile.EndpointUrl),
            new AzureKeyCredential(profile.ApiKey));

        _audioClient = client.GetAudioClient("whisper");
    }

    public async Task<string> TranscribeAsync(byte[] audioData, string language, CancellationToken ct)
    {
        using var audioStream = new MemoryStream(audioData);
        var options = new AudioTranscriptionOptions { Language = language };

        AudioTranscription result = await _audioClient.TranscribeAudioAsync(
            audioStream, "recording.wav", options, ct);

        return result.Text;
    }
}
