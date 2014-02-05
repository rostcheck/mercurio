using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mercurio
{
    public class UnlockCommand : ICommand
    {
        private MainWindowViewModel viewModel;

        public UnlockCommand(MainWindowViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public async void Execute(object parameter)
        {
            // We can't bind to the PasswordBox's SecurePassword; it's a security hole to expose the password
            // (even as a SecureString) through dependency binding, as that is centralized and essentially
            // public. However, we can bind the entire UI control and pass it as a parameter. We do that instead; 
            // it requires the Command to know about the existence of UI controls but it's the best option.     
            PasswordBox passwordBox = parameter as PasswordBox;
            if (passwordBox == null)
                throw new ArgumentException("Invalid PasswordBox sent as parameter to UnlockCommand:Execute()");

            if (viewModel.ValidatePassword(passwordBox.SecurePassword))
            {
                viewModel.StartListener();
            }
            else
            {
                viewModel.PasswordInvalid = true;
                await Task.Delay(1500);
                viewModel.PasswordInvalid = false;
            }
        }
    }
}
