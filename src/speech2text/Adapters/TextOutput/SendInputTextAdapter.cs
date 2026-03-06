using InputSimulatorStandard;
using speech2text.Domain.Ports;

namespace speech2text.Adapters.TextOutput;

public class SendInputTextAdapter : ITextOutput
{
    private readonly InputSimulator _simulator = new();

    public void InjectText(string text)
    {
        _simulator.Keyboard.TextEntry(text);
    }
}
