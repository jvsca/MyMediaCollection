using System;
using System.Windows.Input;

namespace MyMediaCollection.ViewModels
{
    public class RelayCommand : ICommand
    {

        #region ICommand Implementation

        #region Events

        public event EventHandler CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Methods

        public bool CanExecute(object parameter) => _CanExecute == null || _CanExecute();
        public void Execute(object parameter) => _Action();

        #endregion

        #endregion

        #region Fields

        private readonly Action _Action;
        private readonly Func<bool> _CanExecute;

        #endregion

        #region Constructors

        public RelayCommand(Action action)
            : this(action, null)
        {
        }

        public RelayCommand(Action action, Func<bool> canExecute)
        {
            _Action = action ?? throw new ArgumentNullException(nameof(action));
            _CanExecute = canExecute;
        }

        #endregion
    }
}
