#region usings

using Microsoft.Extensions.DependencyInjection;

using MyMediaCollection.Model;

using Windows.Storage;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

#endregion

namespace MyMediaCollection.Views
{

    public sealed partial class ItemDetailsPage : Page
    {

        #region Constructors

        public ItemDetailsPage()
        {
            InitializeComponent();

            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            // Load the user setting.
            var haveExplainedSaveSetting = localSettings.Values[nameof(SavingTip)] as string;
            // If the user has not seen the save tip, display it.
            if (!bool.TryParse(haveExplainedSaveSetting, out var result) || !result)
            {
                SavingTip.IsOpen = true;
                // Save the teaching tip setting.
                localSettings.Values[nameof(SavingTip)] = "true";
            }
        }

        #endregion

        #region Properties

        public ItemDetailsViewModel ViewModel { get; } = (Application.Current as App)?.Container.GetService<ItemDetailsViewModel>();

        #endregion

        #region Public Methods

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var selectedItemId = (int)e.Parameter;
            if (selectedItemId > 0)
            {
                ViewModel.InitializeItemDetailData(selectedItemId);
            }
        }

        #endregion
    }
}
