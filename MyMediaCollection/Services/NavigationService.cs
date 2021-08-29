#region Usings

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using MyMediaCollection.Interfaces;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

#endregion

namespace MyMediaCollection.Services
{
    public class NavigationService : INavigationService
    {

        #region INavigationService Implementation

        #region Properties

        /// <summary>
        /// Gets the name of the currently displayed page.
        /// </summary>
        public string CurrentPage
        {
            get
            {
                Frame frame = AppFrame;

                if (frame.BackStackDepth == 0)
                {
                    return RootPage;
                }

                if (frame.Content == null)
                {
                    return UnknownPage;
                }

                Type type = frame.Content.GetType();

                if (_Pages.Values.All(v => v != type))
                {
                    return UnknownPage;
                }

                KeyValuePair<string, Type> item = _Pages.Single(i => i.Value == type);
                return item.Key;
            }
        }

        #endregion

        #region Methods

        public void NavigateTo(string page, object parameter)
        {
            if (!_Pages.ContainsKey(page))
            {
                throw new ArgumentException($"Unable to find a page registered with the name '{page}'.");
            }
            _ = AppFrame.Navigate(_Pages[page], parameter);
        }

        public void NavigateTo(string page)
        {
            NavigateTo(page, null);
        }

        public void GoBack()
        {
            if (AppFrame?.CanGoBack == true)
            {
                AppFrame.GoBack();
            }
        }

        #endregion

        #endregion

        #region Constants

        public const string RootPage = "(Root)";

        public const string UnknownPage = "(Unknown)";

        #endregion

        #region Fields

        private readonly IDictionary<string, Type> _Pages = new ConcurrentDictionary<string, Type>();

        #endregion

        #region Properties

        private static Frame AppFrame => (Frame)Window.Current.Content;

        //private static Frame AppFrame
        //{
        //    get
        //    {
        //        if (_AppFrame == null)
        //        {
        //            _ = SetAppFrame();
        //        }
        //        return _AppFrame;
        //    }
        //}
        //private static Frame _AppFrame;

        //private static async Task SetAppFrame()
        //{
        //    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
        //                () => { _AppFrame = (Frame)Window.Current.Content; });
        //}

        #endregion

        #region Public Methods

        public void Configure(string page, Type type)
        {
            if (_Pages.Values.Any(v => v == type))
            {
                throw new ArgumentException($"The '{type.Name}' view has already been registered under another name.");
            }
            _Pages[page] = type;
        }

        #endregion
    }
}
