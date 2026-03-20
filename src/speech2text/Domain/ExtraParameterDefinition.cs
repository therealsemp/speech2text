namespace speech2text.Domain;

/// <summary>
/// Describes a backend-specific parameter that a transcription adapter requires beyond
/// the common fields (endpoint, API key, language).
/// Used by the UI to generate form fields dynamically — one TextBox per definition.
/// </summary>
/// <param name="Key">The key used in <see cref="TranscriptionProfile.ExtraParameters"/>.</param>
/// <param name="Label">The human-readable label shown in the settings form.</param>
public record ExtraParameterDefinition(string Key, string Label);
