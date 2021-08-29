#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Dapper;
using Dapper.Contrib.Extensions;

using Microsoft.Data.Sqlite;

using MyMediaCollection.Enums;
using MyMediaCollection.Interfaces;
using MyMediaCollection.Model;

using Windows.Storage;

#endregion

namespace MyMediaCollection.Services
{
    public class SqliteDataService : IDataService
    {

        #region IDataService Implementation

        #region Methods

        public async Task InitializeDataAsync()
        {
            using var connection = await GetOpenConnectionAsync();
            await CreateMediumTableAsync(connection: connection);
            await CreateMediaItemTableAsync(connection: connection);

            PopulateItemTypes();
            await PopulateMediumsAsync(connection: connection);
            PopulateLocationTypes();
        }

        public async Task<IList<MediaItem>> GetItemsAsync()
        {
            using var connection = await GetOpenConnectionAsync();
            return await GetAllMediaItemsAsync(connection);
        }

        public async Task<MediaItem> GetItemAsync(int id)
        {
            IList<MediaItem> mediaItems;

            using (var connection = await GetOpenConnectionAsync())
            {
                mediaItems = await GetAllMediaItemsAsync(connection);
            }

            // Filter the list to get the item for our Id.
            return mediaItems.FirstOrDefault(i => i.Id == id);
        }

        public async Task<int> AddItemAsync(MediaItem item)
        {
            using var connection = await GetOpenConnectionAsync();
            return await InsertMediaItemAsync(connection, item);
        }

        public async Task UpdateItemAsync(MediaItem item)
        {
            using var connection = await GetOpenConnectionAsync();
            await UpdateMediaItemAsync(connection, item);
        }

        public async Task DeleteItemAsync(MediaItem item)
        {
            using var connection = await GetOpenConnectionAsync();
            await DeleteMediaItemAsync(connection, item.Id);
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

        #region Constants

        private const string _DbName = "mediaCollectionData.db";

        #endregion

        #region Fields

        private IList<ItemType> _ItemTypes;
        private IList<Medium> _Mediums;
        private IList<LocationType> _LocationTypes;

        #endregion

        #region Constructors

        public SqliteDataService()
        {

        }

        #endregion

        #region Private Methods

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

        private async Task<SqliteConnection> GetOpenConnectionAsync()
        {
            _ = await ApplicationData.Current.LocalFolder.CreateFileAsync(desiredName: _DbName,
                                                                          options: CreationCollisionOption.OpenIfExists)
                    .AsTask().ConfigureAwait(false);
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, _DbName);
            // dbPath = "C:\Users\jvs_c\AppData\Local\Packages\7af73852-a0eb-4800-92cf-376d2b6c0c0d_795dzahd1109g\LocalState\mediaCollectionData.db"
            var connection = new SqliteConnection(connectionString: $"Filename={dbPath}");
            connection.Open();
            return connection;
        }

        private async Task CreateMediumTableAsync(SqliteConnection connection)
        {
            var tableCommand = @"CREATE TABLE IF NOT EXISTS Mediums
                                 (Id         INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                  Name       NVARCHAR(30) NOT NULL,
                                  MediumType INTEGER NOT NULL)";
            var createTable = new SqliteCommand(commandText: tableCommand, connection: connection);
            _ = await createTable.ExecuteNonQueryAsync();
        }

        private async Task CreateMediaItemTableAsync(SqliteConnection connection)
        {
            var tableCommand = @"CREATE TABLE IF NOT EXISTS MediaItems
                                 (Id           INTEGER PRIMARY KEY AUTOINCREMENT,
                                  Name         NVARCHAR(1000) NOT NULL,
                                  ItemType     INTEGER NOT NULL,
                                  MediumId     INTEGER NOT NULL,
                                  LocationType INTEGER,
                                  CONSTRAINT fk_mediums
                                  FOREIGN KEY(MediumId)
                                  REFERENCES Mediums(Id))";
            var createTable = new SqliteCommand(commandText: tableCommand,
                                                connection: connection);
            _ = await createTable.ExecuteNonQueryAsync();
        }

        private async Task InsertMediumAsync(SqliteConnection connection, Medium medium)
        {
            IEnumerable<long> newIds = await connection.QueryAsync<long>($@"INSERT INTO Mediums
                                                                            ({nameof(medium.Name)}, MediumType)
                                                                            VALUES
                                                                            (@{nameof(medium.Name)}, @{nameof(medium.MediaType)});
                                                                            SELECT last_insert_rowid()",
                                                                         medium);
            medium.Id = (int)newIds.First();
        }

        private async Task<IList<Medium>> GetAllMediumsAsync(SqliteConnection connection)
        {
            IEnumerable<Medium> mediums = await connection.QueryAsync<Medium>(@"SELECT Id,
                                                                                       Name,
                                                                                       MediumType AS MediaType
                                                                                FROM Mediums");
            return mediums.ToList();
        }

        private async Task<List<MediaItem>> GetAllMediaItemsAsync(SqliteConnection connection)
        {
            var itemsResult = await connection.QueryAsync<MediaItem, Medium, MediaItem>
                                (
                                    @"SELECT
                                         [MediaItems].[Id],
                                         [MediaItems].[Name],
                                         [MediaItems].[ItemType] AS MediaType,
                                         [MediaItems].[LocationType] AS Location,
                                         [Mediums].[Id],
                                         [Mediums].[Name],
                                         [Mediums].[MediumType] AS MediaType
                                      FROM [MediaItems]
                                      JOIN [Mediums]
                                        ON [Mediums].[Id] = [MediaItems].[MediumId]",
                                 (item, medium) =>
                                                     {
                                                         item.MediumInfo = medium;
                                                         return item;
                                                     }
                                 );
            return itemsResult.ToList();
        }

        private async Task<int> InsertMediaItemAsync(SqliteConnection connection, MediaItem item)
        {
            var newIds = await connection.QueryAsync<long>(
                            @"INSERT INTO MediaItems
                              (Name, ItemType, MediumId, LocationType)
                              VALUES
                              (@Name, @MediaType, @MediumId, @Location);
                              SELECT last_insert_rowid()", item);
            return (int)newIds.First();
        }

        private async Task UpdateMediaItemAsync(SqliteConnection db, MediaItem item)
        {
            _ = await db.QueryAsync(
                    @"UPDATE MediaItems
                      SET Name = @Name,
                          ItemType = @MediaType,
                          MediumId = @MediumId,
                          LocationType = @Location
                      WHERE Id = @Id;", item);
        }

        private async Task DeleteMediaItemAsync(SqliteConnection connection, int id)
        {
            _ = await connection.DeleteAsync(new MediaItem { Id = id });
        }

        private async Task PopulateMediumsAsync(SqliteConnection connection)
        {
            _Mediums = await GetAllMediumsAsync(connection);
            if (_Mediums.Count == 0)
            {
                var cd = new Medium { Id = 1, MediaType = ItemType.Music, Name = "CD" };
                var vinyl = new Medium { Id = 2, MediaType = ItemType.Music, Name = "Vinyl" };
                var hardcover = new Medium { Id = 3, MediaType = ItemType.Book, Name = "Hardcover" };
                var paperback = new Medium { Id = 4, MediaType = ItemType.Book, Name = "Paperback" };
                var dvd = new Medium { Id = 5, MediaType = ItemType.Video, Name = "DVD" };
                var bluRay = new Medium { Id = 6, MediaType = ItemType.Video, Name = "Blu Ray" };

                var mediums = new List<Medium>
                                {
                                    cd,
                                    vinyl,
                                    hardcover,
                                    paperback,
                                    dvd,
                                    bluRay
                                };

                foreach (var medium in mediums)
                {
                    await InsertMediumAsync(connection, medium);
                }

                _Mediums = await GetAllMediumsAsync(connection);
            }
        }

        #endregion

        #region Public Methods

        #endregion

    }
}
