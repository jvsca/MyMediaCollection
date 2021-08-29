#region Usings

using System.Collections.Generic;
using System.Threading.Tasks;

using MyMediaCollection.Enums;
using MyMediaCollection.Model;

#endregion

namespace MyMediaCollection.Interfaces
{
    public interface IDataService
    {

        #region Methods

        Task InitializeDataAsync();
        Task<IList<MediaItem>> GetItemsAsync();
        Task<MediaItem> GetItemAsync(int id);
        Task<int> AddItemAsync(MediaItem item);
        Task UpdateItemAsync(MediaItem item);
        Task DeleteItemAsync(MediaItem item);
        IList<ItemType> GetItemTypes();
        Medium GetMedium(string name);
        Medium GetMedium(int id);
        IList<Medium> GetMediums();
        IList<Medium> GetMediums(ItemType itemType);
        IList<LocationType> GetLocationTypes();

        #endregion

    }
}
