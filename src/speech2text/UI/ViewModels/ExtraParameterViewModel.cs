namespace speech2text.UI.ViewModels;

/// <summary>
/// Wraps one <see cref="Domain.ExtraParameterDefinition"/> with its current value,
/// for display as a dynamically generated TextBox in the settings form.
/// </summary>
public class ExtraParameterViewModel(string key, string label, string value) : ViewModelBase
{
    private string _value = value;

    public string Key   { get; } = key;
    public string Label { get; } = label;

    public string Value
    {
        get => _value;
        set => SetField(ref _value, value);
    }
}
