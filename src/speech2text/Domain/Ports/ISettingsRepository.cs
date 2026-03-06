namespace speech2text.Domain.Ports;

public interface ISettingsRepository
{
    AppSettings Load();
    void Save(AppSettings settings);
}
