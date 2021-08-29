namespace MyMediaCollection.Interfaces
{
    public interface INavigationService
    {

        #region Properties
        string CurrentPage { get; }

        #endregion

        #region Methods

        void NavigateTo(string page);
        void NavigateTo(string page, object parameter);
        void GoBack();

        #endregion

    }
}
