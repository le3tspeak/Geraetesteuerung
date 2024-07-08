using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Übung_Gerät;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // Add Windows Default Drag Function
    [DllImport("user32.dll")]

    // ReSharper disable once InconsistentNaming
    public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

    // Drag window
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        WindowInteropHelper helper = new WindowInteropHelper(this);
        SendMessage(helper.Handle, 161, 2, 0);
    }
}