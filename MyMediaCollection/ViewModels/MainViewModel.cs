#region Usings

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Input;

using MyMediaCollection.Interfaces;
using MyMediaCollection.Model;

using Windows.UI.Xaml.Input;

#endregion

namespace MyMediaCollection.ViewModels
{
    public class MainViewModel : BindableBase
    {

        #region Constants

        private const string _AllMediums = "All";

        #endregion

        #region Fields

        private string _SelectedMedium;
        private ObservableCollection<MediaItem> _Items = new();
        private ObservableCollection<MediaItem> _AllItems;
        private IList<string> _Mediums;
        private MediaItem _SelectedMediaItem;

        #endregion

        #region Constructors

        public MainViewModel(INavigationService navigationService, IDataService dataService)
        {
            _NavigationService = navigationService;
            _DataService = dataService;

            DeleteCommand = new RelayCommand(async () => await DeleteItemAsync(), CanDeleteItem);
            // No CanExecute param is needed for this command
            // because you can always add or edit items.
            AddEditCommand = new RelayCommand(AddOrEditItem);

            PopulateDataAsync();
        }

        #endregion

        #region Properties

        public ObservableCollection<MediaItem> Items
        {
            get => _Items;
            set => SetProperty(ref _Items, value);
        }

        public IList<string> Mediums
        {
            get => _Mediums;
            set => SetProperty(ref _Mediums, value);
        }

        public string SelectedMedium
        {
            get => _SelectedMedium;
            set
            {
                _ = SetProperty(ref _SelectedMedium, value);
                Items.Clear();
                foreach (MediaItem item in _AllItems)
                {
                    if (string.IsNullOrEmpty(_SelectedMedium) ||
                        _SelectedMedium == "All" ||
                        _SelectedMedium == item.MediaType.ToString())
                    {
                        _Items.Add(item);
                    }
                }
            }
        }

        public MediaItem SelectedMediaItem
        {
            get => _SelectedMediaItem;
            set
            {
                _ = SetProperty(ref _SelectedMediaItem, value);
                ((RelayCommand)DeleteCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand AddEditCommand { get; set; }
        public void AddOrEditItem()
        {
            var selectedItemId = -1;
            if (SelectedMediaItem != null)
            {
                selectedItemId = SelectedMediaItem.Id;
            }
            _NavigationService.NavigateTo("ItemDetailsPage", selectedItemId);
        }

        public ICommand DeleteCommand { get; set; }
        public async Task DeleteItemAsync()
        {
            await _DataService.DeleteItemAsync(SelectedMediaItem);

            _ = _AllItems.Remove(SelectedMediaItem);
            _ = _Items.Remove(SelectedMediaItem);
        }

        private bool CanDeleteItem() => SelectedMediaItem != null;

        #endregion

        #region Public Methods

        public async Task PopulateDataAsync()
        {
            _Items.Clear();
            foreach (var item in await _DataService.GetItemsAsync())
            {
                _Items.Add(item);
            }

            _AllItems = new ObservableCollection<MediaItem>(_Items);

            _Mediums = new ObservableCollection<string>
            {
                _AllMediums
            };

            foreach (var itemType in _DataService.GetItemTypes())
            {
                _Mediums.Add(itemType.ToString());
            }

            _SelectedMedium = _Mediums[0];
        }

        [SuppressMessage(category:"Style",
                         checkId: "IDE0060:Remove unused parameter", 
                         Justification = "Part of the public API.")]
        public void ListViewDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            AddOrEditItem();
        }

        #endregion
    }
}
