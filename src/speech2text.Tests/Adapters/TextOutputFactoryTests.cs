using speech2text.Adapters.TextOutput;
using speech2text.Domain;

namespace speech2text.Tests.Adapters;

public class TextOutputFactoryTests
{
    private readonly TextOutputFactory _factory = new();

    [Fact]
    public void Create_SendInput_ReturnsSendInputTextAdapter()
    {
        var output = _factory.Create(TextInsertionMode.SendInput);

        Assert.IsType<SendInputTextAdapter>(output);
    }

    [Fact]
    public void Create_ClipboardPaste_ReturnsClipboardPasteTextAdapter()
    {
        var output = _factory.Create(TextInsertionMode.ClipboardPaste);

        Assert.IsType<ClipboardPasteTextAdapter>(output);
    }

    [Fact]
    public void Create_UnknownMode_ThrowsNotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() => _factory.Create((TextInsertionMode)999));
    }
}
