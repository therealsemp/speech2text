namespace speech2text.Domain.Ports;

public interface ITextOutput
{
    void InjectText(string text);
}
