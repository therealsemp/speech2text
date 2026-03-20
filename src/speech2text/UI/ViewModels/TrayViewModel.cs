using speech2text.UI.Views;

namespace speech2text.UI.ViewModels;

public class TrayViewModel
{
    public RelayCommand OpenOverlayCommand  { get; }
    public RelayCommand OpenSettingsCommand { get; }
    public RelayCommand ExitCommand         { get; }

    public TrayViewModel(OverlayWindow overlay, SettingsWindow settings)
    {
        OpenOverlayCommand  = new RelayCommand(() => { overlay.Show();   overlay.Activate(); });
        OpenSettingsCommand = new RelayCommand(() => { settings.Show();  settings.Activate(); });
        ExitCommand         = new RelayCommand(() => System.Windows.Application.Current.Shutdown());
    }
}
