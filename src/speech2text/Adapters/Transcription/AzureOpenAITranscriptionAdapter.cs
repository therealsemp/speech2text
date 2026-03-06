using System.IO;
using Azure;
using Azure.AI.OpenAI;
using OpenAI.Audio;
using speech2text.Domain;
using speech2text.Domain.Ports;

namespace speech2text.Adapters.Transcription;

/// <summary>
/// Transcribes audio using the Azure OpenAI Whisper model via the Azure.AI.OpenAI SDK (v2).
/// Creates an <see cref="AzureOpenAIClient"/> from the profile's endpoint URL and API key,
/// obtains an <see cref="OpenAI.Audio.AudioClient"/> for the "whisper" deployment,
/// and calls TranscribeAudioAsync with the WAV audio data and target language.
/// A new client is created per call — connection pooling is handled internally by the SDK.
/// </summary>
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
