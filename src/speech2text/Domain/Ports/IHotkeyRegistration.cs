namespace speech2text.Domain.Ports;

public interface IHotkeyRegistration
{
    void Register(string hotkey, Action onTriggered);
    void Unregister(string hotkey);
}
