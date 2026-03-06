using speech2text.Domain;
using speech2text.Domain.Ports;

namespace speech2text.Adapters.Transcription;

/// <summary>
/// Creates the appropriate <see cref="ITranscriptionBackend"/> based on the service type
/// declared in the transcription profile. Uses a switch expression on <see cref="TranscriptionServiceType"/>.
/// To add a new backend: add an enum value, implement a new adapter, and add one case here.
///
/// Tracks the last created adapter and disposes it before creating a new one, preventing
/// TCP socket exhaustion when the user switches profiles.
/// </summary>
public class TranscriptionBackendFactory : ITranscriptionBackendFactory, IDisposable
{
    private IDisposable? _currentAdapter;

    public ITranscriptionBackend Create(TranscriptionProfile profile)
    {
        _currentAdapter?.Dispose();

        var adapter = profile.ServiceType switch
        {
            TranscriptionServiceType.AzureOpenAI => new AzureOpenAITranscriptionAdapter(profile),
            _ => throw new NotSupportedException($"Unsupported service type: {profile.ServiceType}")
        };

        _currentAdapter = adapter;
        return adapter;
    }

    public void Dispose()
    {
        _currentAdapter?.Dispose();
        _currentAdapter = null;
    }
}
