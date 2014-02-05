using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Entities;
using MercurioAppServiceLayer;
using MercurioUIControls;

namespace Mercurio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml. Thin UI; should only do presentation.
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int messagePoolTimeInMS = 1000;
        private const string title = "Mercurio Secure Communicator";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //appServiceLayer.StopListener();
        }

        // Need to do this in the code-behind, can't bind it, because PasswordBox's SecurePassword
        // is not a DependencyProperty
        private void pbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (pbPassword.SecurePassword.Length > 0)
                btnOk.IsEnabled = true;
            else
                btnOk.IsEnabled = false;
        }
    }
}
