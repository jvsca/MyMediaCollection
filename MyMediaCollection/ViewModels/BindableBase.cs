#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

using MyMediaCollection.Interfaces;

#endregion

namespace MyMediaCollection.ViewModels
{
    public class BindableBase : INotifyPropertyChanged, INotifyDataErrorInfo, IValidatable
    {

        #region INotifyPropertyChanged Implementation

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged(string propertyName, object value)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Validate(propertyName, value);
        }

        #endregion

        #endregion

        #region INotifyDataErrorInfo Implementation

        #region Events

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        #endregion

        #region Properties

        public bool HasErrors => _Errors.Any();

        #endregion

        #region Methods

        public IEnumerable GetErrors(string propertyName)
        {
            return _Errors[propertyName];
        }

        #endregion

        #endregion

        #region IValidatable Implementation

        #region Methods

        public void Validate(string memberName, object value)
        {
            ClearErrors(memberName);
            List<ValidationResult> results = new();
            bool result = Validator.TryValidateProperty(value,
                                                        new ValidationContext(this, null, null) { MemberName = memberName },
                                                        results);
            if (!result)
            {
                AddErrors(memberName, results);
            }
        }

        #endregion

        #endregion

        #region Fields

        private readonly Dictionary<string, List<ValidationResult>> _Errors = new();
        protected INavigationService _NavigationService;
        protected IDataService _DataService;

        #endregion

        #region Private Methods

        private void ClearErrors(string propertyName)
        {
            if (_Errors.TryGetValue(propertyName, out List<ValidationResult> errors))
            {
                errors.Clear();
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        private void AddErrors(string propertyName, IEnumerable<ValidationResult> results)
        {
            if (!_Errors.TryGetValue(propertyName, out List<ValidationResult> errors))
            {
                errors = new List<ValidationResult>();
                _Errors.Add(propertyName, errors);
            }

            errors.AddRange(results);
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        #endregion

        #region Public Methods

        protected bool SetProperty<T>(ref T originalValue,
                                      T newValue,
                                      [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(originalValue, newValue))
            {
                return false;
            }
            originalValue = newValue;
            OnPropertyChanged(propertyName, newValue);
            return true;
        }

        #endregion

    }
}
