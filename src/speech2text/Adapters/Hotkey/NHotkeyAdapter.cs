using System.Windows.Input;
using NHotkey;
using NHotkey.Wpf;
using speech2text.Domain.Ports;

namespace speech2text.Adapters.Hotkey;

public class NHotkeyAdapter : IHotkeyRegistration
{
    public void Register(string hotkey, Action onTriggered)
    {
        var (modifiers, key) = ParseHotkey(hotkey);
        HotkeyManager.Current.AddOrReplace(hotkey, key, modifiers, (_, _) => onTriggered());
    }

    public void Unregister(string hotkey)
    {
        HotkeyManager.Current.Remove(hotkey);
    }

    private static (ModifierKeys modifiers, Key key) ParseHotkey(string hotkey)
    {
        var modifiers = ModifierKeys.None;
        var key = Key.None;

        foreach (var part in hotkey.Split('+'))
        {
            switch (part.Trim().ToLowerInvariant())
            {
                case "ctrl":  modifiers |= ModifierKeys.Control; break;
                case "shift": modifiers |= ModifierKeys.Shift;   break;
                case "alt":   modifiers |= ModifierKeys.Alt;     break;
                case "win":   modifiers |= ModifierKeys.Windows; break;
                default:
                    Enum.TryParse(part.Trim(), ignoreCase: true, out key);
                    break;
            }
        }

        return (modifiers, key);
    }
}
