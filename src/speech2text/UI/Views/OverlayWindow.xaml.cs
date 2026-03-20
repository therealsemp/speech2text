using System.Runtime.InteropServices;
using System.Windows;
using ButtonBase = System.Windows.Controls.Primitives.ButtonBase;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using System.Windows.Interop;
using System.Windows.Media;
using speech2text.UI.ViewModels;

namespace speech2text.UI.Views;

public partial class OverlayWindow : Window
{
    private readonly OverlayViewModel _viewModel;

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

    public OverlayWindow(OverlayViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        // Drag only when clicking in the title bar area (top 34 px)
        if (e.GetPosition(this).Y > 34) return;

        // Don't drag if the click originated on a button (minimize / close)
        if (IsInsideButton(e.OriginalSource as DependencyObject)) return;

        ReleaseMouseCapture();
        SendMessage(new WindowInteropHelper(this).Handle, 0x00A1, 2, 0); // WM_NCLBUTTONDOWN, HTCAPTION
    }

    private static bool IsInsideButton(DependencyObject? element)
    {
        while (element is not null)
        {
            if (element is ButtonBase) return true;
            element = VisualTreeHelper.GetParent(element);
        }
        return false;
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            _viewModel.HandleEscapeKey();
            e.Handled = true;
        }
    }
}
