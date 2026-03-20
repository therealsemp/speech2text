namespace speech2text.Domain;

/// <summary>
/// Root entity holding the full application configuration.
/// Loaded at startup and persisted whenever the user changes a setting.
///
/// Contains the list of transcription profiles, the currently active one,
/// the selected audio input device, and the global hotkey binding.
/// </summary>
public class AppSettings
{
    /// <summary>Id of the profile currently selected for transcription.</summary>
    public Guid ActiveProfileId { get; set; }

    /// <summary>Microphone to record from. Null means the system default device is used.</summary>
    public AudioDevice? SelectedAudioDevice { get; set; }

    /// <summary>Global hotkey that toggles recording. Defaults to Ctrl+Shift+R.</summary>
    public string HotkeyBinding { get; set; } = "Ctrl+Shift+R";

    /// <summary>All configured transcription profiles. At least one is expected at runtime.</summary>
    public List<TranscriptionProfile> Profiles { get; set; } = [];
}
