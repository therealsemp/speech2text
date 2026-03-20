using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace speech2text.Domain;

/// <summary>
/// A named configuration for a transcription service. Each profile targets one backend
/// (e.g. Azure OpenAI Whisper) with its own credentials and language setting.
///
/// Typical usage: one profile per language, even when pointing to the same backend.
/// One profile is marked active at any time via <see cref="AppSettings.ActiveProfileId"/>.
/// </summary>
public class TranscriptionProfile : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private string _apiKey = string.Empty;
    private string _endpointUrl = string.Empty;
    private string _language = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>Display name shown in the overlay and settings panel.</summary>
    public string Name
    {
        get => _name;
        set { if (_name != value) { _name = value; OnPropertyChanged(); } }
    }

    /// <summary>Determines which adapter the factory will instantiate for this profile.</summary>
    public TranscriptionServiceType ServiceType { get; set; } = TranscriptionServiceType.AzureOpenAI;

    /// <summary>API key for authenticating with the transcription service.</summary>
    public string ApiKey
    {
        get => _apiKey;
        set { if (_apiKey != value) { _apiKey = value; OnPropertyChanged(); } }
    }

    /// <summary>Service endpoint URL (required for Azure OpenAI deployments).</summary>
    public string EndpointUrl
    {
        get => _endpointUrl;
        set { if (_endpointUrl != value) { _endpointUrl = value; OnPropertyChanged(); } }
    }

    /// <summary>BCP-47 language code passed to the transcription service (e.g. "fr", "en-US").</summary>
    public string Language
    {
        get => _language;
        set { if (_language != value) { _language = value; OnPropertyChanged(); } }
    }

    /// <summary>
    /// Backend-specific parameters that don't fit the common fields above.
    /// Examples: deployment name, model size for local Whisper, project ID for Google Speech.
    /// Read by the adapter via <c>ExtraParameters.GetValueOrDefault("key")</c>.
    /// Empty by default — unused in v1 (Azure OpenAI only).
    /// </summary>
    public Dictionary<string, string> ExtraParameters { get; set; } = [];
}
