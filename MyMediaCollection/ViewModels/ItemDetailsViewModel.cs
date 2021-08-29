#region Usings

using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using MyMediaCollection.Enums;
using MyMediaCollection.Interfaces;
using MyMediaCollection.ViewModels;

#endregion

namespace MyMediaCollection.Model
{
    public class ItemDetailsViewModel : BindableBase
    {

        #region Fields

        private ObservableCollection<string> _LocationTypes = new();
        private ObservableCollection<string> _Mediums = new();
        private ObservableCollection<string> _ItemTypes = new();

        private int _ItemId;
        private int _SelectedItemId = -1;

        #endregion

        #region Constructors

        public ItemDetailsViewModel(INavigationService navigationService, IDataService dataService)
        {
            _NavigationService = navigationService;
            _DataService = dataService;

            CancelCommand = new RelayCommand(Cancel);

            PopulateLists();
            _ = PopulateExistingItemAsync(dataService);
            IsDirty = false;
        }

        #endregion

        #region Properties

        public ICommand CancelCommand { get; set; }

        [MinLength(2, ErrorMessage = "Item name must be at least 2 characters.")]
        [MaxLength(100, ErrorMessage = "Item name must be 100 characters or less.")]
        public string ItemName
        {
            get => _ItemName;
            set
            {
                if (!SetProperty(ref _ItemName, value, nameof(ItemName)))
                {
                    return;
                }
                IsDirty = true;
            }
        }
        private string _ItemName;

        public string SelectedMedium
        {
            get => _SelectedMedium;
            set
            {
                if (!SetProperty(ref _SelectedMedium, value, nameof(SelectedMedium)))
                {
                    return;
                }
                IsDirty = true;
            }
        }
        private string _SelectedMedium;

        public string SelectedItemType
        {
            get => _SelectedItemType;
            set
            {
                if (!SetProperty(ref _SelectedItemType, value, nameof(SelectedItemType)))
                {
                    return;
                }

                IsDirty = true;

                Mediums.Clear();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    foreach (string med in _DataService.GetMediums((ItemType)Enum.Parse(typeof(ItemType), SelectedItemType)).Select(m => m.Name))
                    {
                        Mediums.Add(med);
                    }
                }
            }
        }
        private string _SelectedItemType;

        public string SelectedLocation
        {
            get => _SelectedLocation;
            set
            {
                if (!SetProperty(ref _SelectedLocation, value, nameof(SelectedLocation)))
                {
                    return;
                }
                IsDirty = true;
            }
        }
        private string _SelectedLocation;

        public ObservableCollection<string> LocationTypes
        {
            get => _LocationTypes;
            set => SetProperty(ref _LocationTypes, value, nameof(LocationTypes));
        }

        public ObservableCollection<string> Mediums
        {
            get => _Mediums;
            set => SetProperty(ref _Mediums, value, nameof(Mediums));
        }

        public ObservableCollection<string> ItemTypes
        {
            get => _ItemTypes;
            set => SetProperty(ref _ItemTypes, value, nameof(ItemTypes));
        }

        public bool IsDirty
        {
            get => _IsDirty;
            set => SetProperty(ref _IsDirty, value, nameof(IsDirty));
        }
        private bool _IsDirty;

        #endregion  

        #region Private Methods

        private async Task PopulateExistingItemAsync(IDataService dataService)
        {
            if (_SelectedItemId > 0)
            {
                var item = await _DataService.GetItemAsync(_SelectedItemId);
                Mediums.Clear();

                foreach (string medium in dataService.GetMediums(item.MediaType).
                                         Select(m => m.Name))
                {
                    Mediums.Add(medium);
                }

                _ItemId = item.Id;
                ItemName = item.Name;
                SelectedMedium = item.MediumInfo.Name;
                SelectedLocation = item.Location.ToString();
                SelectedItemType = item.MediaType.ToString();
            }
        }

        private void PopulateLists()
        {
            ItemTypes.Clear();
            foreach (var itemType in Enum.GetNames(typeof(ItemType)))
            {
                ItemTypes.Add(itemType);
            }

            LocationTypes.Clear();
            foreach (var locationType in Enum.GetNames(typeof(LocationType)))
            {
                LocationTypes.Add(locationType);
            }

            Mediums = new ObservableCollection<string>();
        }

        private async Task SaveItemAsync()
        {
            MediaItem item;
            if (_ItemId > 0)
            {
                item = await _DataService.GetItemAsync(_ItemId);

                item.Name = ItemName;
                item.Location = (LocationType)Enum.Parse(typeof(LocationType), SelectedLocation);
                item.MediaType = (ItemType)Enum.Parse(typeof(ItemType), SelectedItemType);
                item.MediumInfo = _DataService.GetMedium(SelectedMedium);

                await _DataService.UpdateItemAsync(item);
            }
            else
            {
                item = new MediaItem
                {
                    Name = ItemName,
                    Location = (LocationType)Enum.Parse(typeof(LocationType), SelectedLocation),
                    MediaType = (ItemType)Enum.Parse(typeof(ItemType), SelectedItemType),
                    MediumInfo = _DataService.GetMedium(SelectedMedium)
                };

                _ = await _DataService.AddItemAsync(item);
            }
        }

        public async Task SaveItemAndContinueAsync()
        {
            await SaveItemAsync();
            _ItemId = 0;
            ItemName = "";
            SelectedMedium = null;
            SelectedLocation = null;
            SelectedItemType = null;
            IsDirty = false;
        }

        public async Task SaveItemAndReturnAsync()
        {
            await SaveItemAsync();
            _NavigationService.GoBack();
        }

        private void Cancel()
        {
            _NavigationService.GoBack();
        }

        #endregion

        #region Public Methods

        public void InitializeItemDetailData(int selectedItemId)
        {
            _SelectedItemId = selectedItemId;
            PopulateLists();
            _ = PopulateExistingItemAsync(_DataService);
            IsDirty = false;
        }

        #endregion

    }
}
