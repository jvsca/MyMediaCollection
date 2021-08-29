#region Usings

using System;

using Microsoft.Extensions.DependencyInjection;

using MyMediaCollection.Interfaces;
using MyMediaCollection.Model;
using MyMediaCollection.Services;
using MyMediaCollection.ViewModels;
using MyMediaCollection.Views;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

#endregion

namespace MyMediaCollection
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {

        #region Constructors

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        #endregion

        #region Properties

        /// <summary>
        /// This DI container manages the project's dependencies.
        /// </summary>
        public IServiceProvider Container { get; private set; }

        #endregion

        #region Private Methods

        private IServiceProvider RegisterServices()
        {
            var services = new ServiceCollection();

            var navigationService = new NavigationService();
            navigationService.Configure(nameof(MainPage), typeof(MainPage));
            navigationService.Configure(nameof(ItemDetailsPage), typeof(ItemDetailsPage));

            _ = services.AddSingleton<INavigationService>(navigationService);
            _ = services.AddSingleton<IDataService, SqliteDataService>();

            _ = services.AddTransient<MainViewModel>();
            _ = services.AddTransient<ItemDetailsViewModel>();

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            Container = RegisterServices();
            var dataService = Container.GetService<IDataService>();
            await dataService.InitializeDataAsync();

            var rootFrame = (Frame)Window.Current.Content;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active.
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page.
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application.
                }

                // Place the frame in the current Window.
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter.
                    _ = rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active.
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity.
            deferral.Complete();
        }

        #endregion

    }
}
