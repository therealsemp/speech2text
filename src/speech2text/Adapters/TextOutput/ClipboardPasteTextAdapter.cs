using InputSimulatorStandard;
using InputSimulatorStandard.Native;
using speech2text.Domain.Ports;
using Clipboard = System.Windows.Clipboard;

namespace speech2text.Adapters.TextOutput;

/// <summary>
/// Injects text at the current cursor position by placing it on the clipboard and simulating
/// Ctrl+V, instead of typing it character by character. Faster and more reliable for long text
/// and applications that mangle synthetic keystrokes, at the cost of overwriting the clipboard
/// (the previous clipboard content is not restored — see CLAUDE.md decision).
/// Windows-only constraint: relies on the WPF clipboard and the Win32 SendInput API.
/// </summary>
public class ClipboardPasteTextAdapter : ITextOutput
{
    private readonly InputSimulator _simulator = new();

    public void InjectText(string text)
    {
        Clipboard.SetText(text);
        _simulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.LCONTROL, VirtualKeyCode.VK_V);
    }
}
