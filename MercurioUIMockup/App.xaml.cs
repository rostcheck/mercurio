using MercurioAppServiceLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MercurioUIMockup
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Initialize main window and app service; hook them together
            MainWindow mainWindow = new MainWindow();
            AppServiceLayer appServiceLayer = new AppServiceLayer(AppCryptoManagerType.GPG);

            // Initialize view model, set it as data context for main window
            StartPageViewModel viewModel = new StartPageViewModel(appServiceLayer);
            mainWindow.DataContext = viewModel;
            //appServiceLayer.StartListener();
            mainWindow.Show();
        }
    }
}
