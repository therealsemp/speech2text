using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using speech2text.UI.ViewModels;

namespace speech2text.UI.Views;

public partial class SettingsWindow : Window
{
    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

    public SettingsWindow(SettingsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.SettingsSaved += _ => Hide();
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        if (e.GetPosition(this).Y > 34) return;
        if (IsInsideButton(e.OriginalSource as DependencyObject)) return;

        ReleaseMouseCapture();
        SendMessage(new WindowInteropHelper(this).Handle, 0x00A1, 2, 0);
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

    private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();
}
