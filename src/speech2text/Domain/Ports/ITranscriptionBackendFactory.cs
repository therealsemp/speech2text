namespace speech2text.Domain.Ports;

/// <summary>
/// Instantiates the correct <see cref="ITranscriptionBackend"/> for a given transcription profile.
/// Acts as the single extension point for adding new transcription providers:
/// adding a backend requires only a new enum value, a new adapter class, and one switch case here.
/// </summary>
public interface ITranscriptionBackendFactory
{
    ITranscriptionBackend Create(TranscriptionProfile profile);

    /// <summary>
    /// Returns the extra parameter definitions required by the adapter for a given service type.
    /// Used by the UI to generate form fields dynamically.
    /// </summary>
    IReadOnlyList<ExtraParameterDefinition> GetParameterDefinitions(TranscriptionServiceType serviceType);
}
