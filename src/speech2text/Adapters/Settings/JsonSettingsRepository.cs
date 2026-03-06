using System.IO;
using System.Text.Json;
using speech2text.Domain;
using speech2text.Domain.Ports;

namespace speech2text.Adapters.Settings;

/// <summary>
/// Persists application settings as a JSON file using System.Text.Json.
/// The file is stored at <c>%AppData%\speech2text\settings.json</c> — the standard location
/// for per-user application data on Windows. The directory is created automatically on first save.
/// Returns a default <see cref="AppSettings"/> instance if the file does not yet exist.
/// </summary>
public class JsonSettingsRepository : ISettingsRepository
{
    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "speech2text",
        "settings.json");

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public AppSettings Load()
    {
        if (!File.Exists(SettingsPath))
            return new AppSettings();

        var json = File.ReadAllText(SettingsPath);
        return JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? new AppSettings();
    }

    public void Save(AppSettings settings)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
        File.WriteAllText(SettingsPath, JsonSerializer.Serialize(settings, JsonOptions));
    }
}
