using System.Windows;
using System.Windows.Input;
using speech2text.Application;
using speech2text.UI.ViewModels;

namespace speech2text.UI.Views;

public partial class OverlayWindow : Window
{
    private readonly RecordingOrchestrator _orchestrator;

    public OverlayWindow(OverlayViewModel viewModel, RecordingOrchestrator orchestrator)
    {
        InitializeComponent();
        DataContext = viewModel;
        _orchestrator = orchestrator;
    }

    private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

    // Escape cancels an in-progress recording when the overlay has focus.
    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            _orchestrator.CancelRecording();
            e.Handled = true;
        }
    }
}
