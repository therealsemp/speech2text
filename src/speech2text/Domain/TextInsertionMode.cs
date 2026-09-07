namespace speech2text.Domain;

/// <summary>
/// Identifies how transcribed text is inserted at the cursor position.
/// The factory reads this value to instantiate the correct <see cref="Ports.ITextOutput"/> adapter.
/// </summary>
public enum TextInsertionMode
{
    /// <summary>Types the text character by character via the Win32 SendInput API.</summary>
    SendInput,

    /// <summary>Places the text on the clipboard and simulates Ctrl+V.</summary>
    ClipboardPaste
}
