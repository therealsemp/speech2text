using System.ClientModel;
using System.IO;
using System.Net.Http;
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
public class AzureOpenAITranscriptionAdapter : ITranscriptionBackend, IDisposable
{
    private readonly AzureOpenAIClient _client;
    private readonly AudioClient _audioClient;
    private bool _disposed;

    public AzureOpenAITranscriptionAdapter(TranscriptionProfile profile)
    {
        if (string.IsNullOrWhiteSpace(profile.EndpointUrl))
            throw new InvalidOperationException("Transcription profile is missing the Endpoint URL. Check your settings.");
        if (string.IsNullOrWhiteSpace(profile.ApiKey))
            throw new InvalidOperationException("Transcription profile is missing the API Key. Check your settings.");
        if (!profile.ExtraParameters.TryGetValue("deploymentName", out var deploymentName) || string.IsNullOrWhiteSpace(deploymentName))
            throw new InvalidOperationException("Transcription profile is missing the Deployment Name. Check your settings.");

        _client = new AzureOpenAIClient(
            new Uri(profile.EndpointUrl),
            new AzureKeyCredential(profile.ApiKey));

        _audioClient = _client.GetAudioClient(deploymentName);
    }

    public async Task<string> TranscribeAsync(byte[] audioData, string language, CancellationToken ct)
    {
        using var audioStream = new MemoryStream(audioData);
        var options = new AudioTranscriptionOptions { Language = language };

        try
        {
            AudioTranscription result = await _audioClient.TranscribeAudioAsync(
                audioStream, "recording.wav", options, ct);

            return result.Text;
        }
        catch (ClientResultException ex) when (ex.Status is 401 or 403)
        {
            throw new InvalidOperationException(
                "Transcription failed: invalid API key or unauthorized access. Check your credentials in Settings.", ex);
        }
        catch (ClientResultException ex) when (ex.Status is 404)
        {
            throw new InvalidOperationException(
                "Transcription failed: endpoint or deployment not found. Check your endpoint URL in Settings.", ex);
        }
        catch (ClientResultException ex)
        {
            throw new InvalidOperationException(
                $"Transcription failed (HTTP {ex.Status}): {ex.Message}", ex);
        }
        catch (RequestFailedException ex) when (ex.Status is 401 or 403)
        {
            throw new InvalidOperationException(
                "Transcription failed: invalid API key or unauthorized access. Check your credentials in Settings.", ex);
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException(
                "Transcription failed: network error. Check your internet connection.", ex);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        (_client as IDisposable)?.Dispose();
    }
}
