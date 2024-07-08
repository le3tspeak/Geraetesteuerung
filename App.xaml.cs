using System.Configuration;
using System.Data;
using System.Windows;

namespace Übung_Gerät;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected void AplicationStart(object sender, StartupEventArgs e)
    {
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
    }
}

