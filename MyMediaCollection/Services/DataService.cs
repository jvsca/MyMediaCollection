#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MyMediaCollection.Enums;
using MyMediaCollection.Interfaces;
using MyMediaCollection.Model;

#endregion

namespace MyMediaCollection.Services
{
    public class DataService : IDataService
    {

        #region IDataService Implementation

        #region Methods

        public async Task InitializeDataAsync()
        {
            PopulateItemTypes();
            PopulateMediums();
            PopulateLocationTypes();
            PopulateItems();
        }

        public async Task<IList<MediaItem>> GetItemsAsync()
        {
            return _Items;
        }

        public async Task<MediaItem> GetItemAsync(int id)
        {
            return _Items.FirstOrDefault(i => i.Id == id);
        }

        public async Task<int> AddItemAsync(MediaItem item)
        {
            item.Id = _Items.Max(i => i.Id) + 1;
            _Items.Add(item);

            return item.Id;
        }

        public async Task UpdateItemAsync(MediaItem item)
        {
            var index = -1;
            var matchedItem = (from x in _Items
                               let ind = index++
                               where x.Id == item.Id
                               select ind).FirstOrDefault();

            if (index == -1)
            {
                throw new Exception($"Unable to update item. Item '{item.Name}' not found in collection.");
            }

            _Items[index] = item;
        }

        public async Task DeleteItemAsync(MediaItem item)
        {
            _Items.Remove(item);
        }

        public IList<ItemType> GetItemTypes()
        {
            return _ItemTypes;
        }

        public Medium GetMedium(string name)
        {
            return _Mediums.FirstOrDefault(m => m.Name == name);
        }

        public Medium GetMedium(int id)
        {
            return _Mediums.FirstOrDefault(m => m.Id == id);
        }

        public IList<Medium> GetMediums()
        {
            return _Mediums;
        }

        public IList<Medium> GetMediums(ItemType itemType)
        {
            return _Mediums
                    .Where(m => m.MediaType == itemType)
                    .ToList();
        }

        public IList<LocationType> GetLocationTypes()
        {
            return _LocationTypes;
        }

        #endregion

        #endregion

        #region Fields

        private IList<MediaItem> _Items;
        private IList<ItemType> _ItemTypes;
        private IList<Medium> _Mediums;
        private IList<LocationType> _LocationTypes;

        #endregion

        #region Private Methods

        private void PopulateItems()
        {
            var cd = new MediaItem
            {
                Id = 1,
                Name = "Classical Favorites",
                MediaType = ItemType.Music,
                MediumInfo = _Mediums.FirstOrDefault(m => m.Name == "CD"),
                Location = LocationType.InCollection
            };

            var book = new MediaItem
            {
                Id = 2,
                Name = "Classic Fairy Tales",
                MediaType = ItemType.Book,
                MediumInfo = _Mediums.FirstOrDefault(m => m.Name == "Hardcover"),
                Location = LocationType.InCollection
            };

            var bluRay = new MediaItem
            {
                Id = 3,
                Name = "The Mummy",
                MediaType = ItemType.Video,
                MediumInfo = _Mediums.FirstOrDefault(m => m.Name == "Blu Ray"),
                Location = LocationType.InCollection
            };

            _Items = new List<MediaItem>
            {
                cd,
                book,
                bluRay
            };
        }

        private void PopulateMediums()
        {
            var cd = new Medium { Id = 1, MediaType = ItemType.Music, Name = "CD" };
            var vinyl = new Medium { Id = 2, MediaType = ItemType.Music, Name = "Vinyl" };
            var hardcover = new Medium { Id = 3, MediaType = ItemType.Book, Name = "Hardcover" };
            var paperback = new Medium { Id = 4, MediaType = ItemType.Book, Name = "Paperback" };
            var dvd = new Medium { Id = 5, MediaType = ItemType.Video, Name = "DVD" };
            var bluRay = new Medium { Id = 6, MediaType = ItemType.Video, Name = "Blu Ray" };

            _Mediums = new List<Medium>
            {
                cd,
                vinyl,
                hardcover,
                paperback,
                dvd,
                bluRay
            };
        }

        private void PopulateItemTypes()
        {
            _ItemTypes = new List<ItemType>
            {
                ItemType.Book,
                ItemType.Music,
                ItemType.Video
            };
        }

        private void PopulateLocationTypes()
        {
            _LocationTypes = new List<LocationType>
            {
                LocationType.InCollection,
                LocationType.Loaned
            };
        }

        #endregion

    }
}
