using System.Configuration;
using System.Data;
using System.Windows;
using Application = System.Windows.Application;

namespace InteractiveDisplayCapture
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1?? Start floating edge handle
            var mainWindow = new MainWindow();
            mainWindow.Show();

        }
    }
}
