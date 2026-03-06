using System.Windows.Input;
using NHotkey;
using NHotkey.Wpf;
using speech2text.Domain.Ports;

namespace speech2text.Adapters.Hotkey;

/// <summary>
/// Implements global hotkey registration using the Win32 <c>RegisterHotKey</c> / <c>UnregisterHotKey</c> API
/// via the NHotkey.Wpf library.
///
/// A global hotkey fires system-wide regardless of which application has focus, unlike WPF's built-in
/// KeyBinding which only works when the window is in the foreground.
///
/// NHotkey.Wpf bridges the gap by using the WPF main window as the receiver of Win32 <c>WM_HOTKEY</c>
/// messages, translating them into C# events. This requires an active WPF message loop.
///
/// Windows-only constraint: RegisterHotKey is a Win32 API, consistent with the overall WPF/Windows stack.
///
/// Note: if the requested hotkey is already registered by another application,
/// NHotkey throws <see cref="HotkeyAlreadyRegisteredException"/> — to be handled in Phase 6.
/// </summary>
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
