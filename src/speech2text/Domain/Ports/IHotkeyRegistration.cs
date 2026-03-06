namespace speech2text.Domain.Ports;

/// <summary>
/// Registers and unregisters system-wide keyboard shortcuts.
/// A global hotkey fires regardless of which application currently has focus,
/// making it the entry point for starting and stopping a recording session from anywhere on the desktop.
/// Hotkeys are expressed as human-readable strings, e.g. "Ctrl+Shift+R".
/// </summary>
public interface IHotkeyRegistration
{
    void Register(string hotkey, Action onTriggered);
    void Unregister(string hotkey);
}
