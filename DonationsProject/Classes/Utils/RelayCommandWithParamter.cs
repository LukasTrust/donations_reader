using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DonationsProject.Classes.Utils
{
    public class RelayCommandWithParamter<T> : ICommand
    {
        private readonly Action<T> _execute;

        public RelayCommandWithParamter(Action<T> execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _execute((T)parameter);

        public event EventHandler CanExecuteChanged;
    }
}
