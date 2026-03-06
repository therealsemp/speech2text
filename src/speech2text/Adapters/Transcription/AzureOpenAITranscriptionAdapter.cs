using System.IO;
using Azure;
using Azure.AI.OpenAI;
using OpenAI.Audio;
using speech2text.Domain;
using speech2text.Domain.Ports;

namespace speech2text.Adapters.Transcription;

public class AzureOpenAITranscriptionAdapter(TranscriptionProfile profile) : ITranscriptionBackend
{
    public async Task<string> TranscribeAsync(byte[] audioData, string language, CancellationToken ct)
    {
        var client = new AzureOpenAIClient(
            new Uri(profile.EndpointUrl),
            new AzureKeyCredential(profile.ApiKey));

        AudioClient audioClient = client.GetAudioClient("whisper");

        using var audioStream = new MemoryStream(audioData);
        var options = new AudioTranscriptionOptions { Language = language };

        AudioTranscription result = await audioClient.TranscribeAudioAsync(
            audioStream, "recording.wav", options, ct);

        return result.Text;
    }
}
