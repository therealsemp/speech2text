namespace speech2text.Domain;

/// <summary>
/// A named configuration for a transcription service. Each profile targets one backend
/// (e.g. Azure OpenAI Whisper) with its own credentials and language setting.
///
/// Typical usage: one profile per language, even when pointing to the same backend.
/// One profile is marked active at any time via <see cref="AppSettings.ActiveProfileId"/>.
/// </summary>
public class TranscriptionProfile
{
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>Display name shown in the overlay and settings panel.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Determines which adapter the factory will instantiate for this profile.</summary>
    public TranscriptionServiceType ServiceType { get; set; } = TranscriptionServiceType.AzureOpenAI;

    /// <summary>API key for authenticating with the transcription service.</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Service endpoint URL (required for Azure OpenAI deployments).</summary>
    public string EndpointUrl { get; set; } = string.Empty;

    /// <summary>BCP-47 language code passed to the transcription service (e.g. "fr", "en-US").</summary>
    public string Language { get; set; } = string.Empty;

    /// <summary>
    /// Backend-specific parameters that don't fit the common fields above.
    /// Examples: deployment name, model size for local Whisper, project ID for Google Speech.
    /// Read by the adapter via <c>ExtraParameters.GetValueOrDefault("key")</c>.
    /// Empty by default — unused in v1 (Azure OpenAI only).
    /// </summary>
    public Dictionary<string, string> ExtraParameters { get; set; } = [];
}
