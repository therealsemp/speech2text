namespace speech2text.Domain.Ports;

/// <summary>
/// Persists and retrieves application settings (transcription profiles, active profile,
/// selected audio device, hotkey binding).
/// Abstracts the storage mechanism — the domain only works with <see cref="AppSettings"/>,
/// unaware of whether data is stored in a file, a database, or elsewhere.
/// </summary>
public interface ISettingsRepository
{
    AppSettings Load();
    void Save(AppSettings settings);
}
