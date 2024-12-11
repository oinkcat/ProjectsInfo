using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjectsInfo.ViewModels
{
    /// <summary>
    /// Команда, представляющая действие
    /// </summary>
    internal class ActionCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly Action<object> commandAction;

        private bool isEnabled;

        /// <summary>
        /// Активна ли команда
        /// </summary>
        public bool IsEnabled
        {
            get => isEnabled;

            set
            {
                isEnabled = value;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public ActionCommand(Action<object> action, bool enabled = true)
        {
            commandAction = action;
            IsEnabled = enabled;
        }

        /// <inheritdoc />
        public bool CanExecute(object parameter) => IsEnabled;

        /// <inheritdoc />
        public void Execute(object parameter) => commandAction(parameter);
    }
}
