using speech2text.Domain;
using speech2text.Domain.Ports;

namespace speech2text.Adapters.Transcription;

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
