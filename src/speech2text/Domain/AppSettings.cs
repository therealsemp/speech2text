namespace speech2text.Domain;

public class AppSettings
{
    public Guid ActiveProfileId { get; set; }
    public AudioDevice? SelectedAudioDevice { get; set; }
    public string HotkeyBinding { get; set; } = "Ctrl+Shift+R";
    public List<TranscriptionProfile> Profiles { get; set; } = [];
}
