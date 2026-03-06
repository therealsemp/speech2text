using speech2text.Domain;
using speech2text.Domain.Ports;

namespace speech2text.Adapters.Transcription;

/// <summary>
/// Creates the appropriate <see cref="ITranscriptionBackend"/> based on the service type
/// declared in the transcription profile. Uses a switch expression on <see cref="TranscriptionServiceType"/>.
/// To add a new backend: add an enum value, implement a new adapter, and add one case here.
/// </summary>
public class TranscriptionBackendFactory : ITranscriptionBackendFactory
{
    public ITranscriptionBackend Create(TranscriptionProfile profile)
    {
        return profile.ServiceType switch
        {
            TranscriptionServiceType.AzureOpenAI => new AzureOpenAITranscriptionAdapter(profile),
            _ => throw new NotSupportedException($"Unsupported service type: {profile.ServiceType}")
        };
    }
}
