#region usings

using Microsoft.Extensions.DependencyInjection;

using MyMediaCollection.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#endregion

namespace MyMediaCollection
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        #region Constructors

        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        #endregion

        #region Properties

        public MainViewModel ViewModel { get; } = (Application.Current as App).Container.GetService<MainViewModel>();

        #endregion

        #region Private Methods

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        #endregion

    }
}
