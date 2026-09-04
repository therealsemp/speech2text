using speech2text.Domain;
using speech2text.Domain.Ports;

namespace speech2text.Adapters.TextOutput;

/// <summary>
/// Creates the appropriate <see cref="ITextOutput"/> based on the configured <see cref="TextInsertionMode"/>.
/// Uses a switch expression, mirroring <see cref="Transcription.TranscriptionBackendFactory"/>.
/// To add a new insertion mode: add an enum value, implement a new adapter, and add one case here.
/// </summary>
public class TextOutputFactory : ITextOutputFactory
{
    public ITextOutput Create(TextInsertionMode mode) =>
        mode switch
        {
            TextInsertionMode.SendInput      => new SendInputTextAdapter(),
            TextInsertionMode.ClipboardPaste => new ClipboardPasteTextAdapter(),
            _ => throw new NotSupportedException($"Unsupported text insertion mode: {mode}")
        };
}
