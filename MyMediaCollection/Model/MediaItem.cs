#region Usings

using Dapper.Contrib.Extensions;

using MyMediaCollection.Enums;

#endregion

namespace MyMediaCollection.Model
{
    public class MediaItem
    {

        #region Properties

        public int MediumId => MediumInfo.Id;

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ItemType MediaType { get; set; }

        [Computed]
        public Medium MediumInfo { get; set; }
        public LocationType Location { get; set; }

        #endregion

    }
}
