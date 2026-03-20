namespace speech2text.Domain;

/// <summary>
/// Identifies the speech-to-text provider used by a <see cref="TranscriptionProfile"/>.
/// The factory reads this value to instantiate the correct adapter.
/// Adding a new provider requires: a new enum value here, a new adapter class, and one switch case in the factory.
/// </summary>
public enum TranscriptionServiceType
{
    AzureOpenAI
}
