namespace speech2text.Domain.Ports;

/// <summary>
/// Creates the appropriate <see cref="ITextOutput"/> adapter based on the configured
/// <see cref="TextInsertionMode"/>. Only one implementation is active for a given insertion.
/// </summary>
public interface ITextOutputFactory
{
    ITextOutput Create(TextInsertionMode mode);
}
