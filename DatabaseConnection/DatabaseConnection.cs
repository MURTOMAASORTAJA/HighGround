using Data.Interfaces;
using MongoDB.Driver;

namespace HighGround
{
    public class DatabaseConnection
    {
        public MongoClient MongoClient { get; }
        private readonly string dbName;

        public DatabaseConnection(string server, int port, string username, string password, string db)
        {
            MongoClient = new MongoClient(GetMongoClientSettings(server, port, username, password, db));
            dbName = db;
        }

        /// <summary>
        /// Generates MongoClientSettings object from parameters.
        /// </summary>
        /// <param name="server">MongoDB server host.</param>
        /// <param name="port">MongoDB server port.</param>
        /// <param name="username">MongoDB username.</param>
        /// <param name="password">MongoDB password.</param>
        /// <param name="db">MongoDB database name.</param>
        public static MongoClientSettings GetMongoClientSettings(string server, int port, string username, string password, string db)
        {
            return new MongoClientSettings()
            {
                Credential = MongoCredential.CreateCredential(db, username, password),
                Server = new MongoServerAddress(server, port)
            };
        }

        public T? Get<T>(Guid id)
        {
            var type = typeof(T);
            if (!(type.GetInterfaces().Contains(typeof(IIdentifiable))))
            {
                throw new ArgumentException("That object type is not an Identifiable.");
            }

            var collectionName = $"{type.Name.ToLowerInvariant()}s";
            collectionName = collectionName switch
            {
                "persons" => "people",
                _ => collectionName
            };
            
            return GetDatabase()
                .GetCollection<T>(collectionName)
                .AsQueryable()
                .Where(i => ((IIdentifiable)i).Id == id)
                .FirstOrDefault();
        }

        public IQueryable<T> Query<T>()
        {
            
            var collectionName = $"{typeof(T).Name.ToLowerInvariant()}s";
            collectionName = collectionName switch
            {
                "persons" => "people",
                _ => collectionName
            };
            if (collectionName.EndsWith("ss"))
            {
                collectionName = collectionName.Substring(0, collectionName.Length - 1);
            }

            return GetDatabase()
                .GetCollection<T>(collectionName)
                .AsQueryable();
        }

        public bool Update<T>(Guid id, T newObj)
        {
            T oldObj;
            var type = typeof(T);
            if (!type.GetInterfaces().Contains(typeof(IIdentifiable)))
            {
                throw new ArgumentException("Object is not identifiable.");
            }

            oldObj = Query<T>().Where(o => ((IIdentifiable)o).Id == id).FirstOrDefault();
            if (oldObj == null)
            {
                throw new FileNotFoundException($"No existing {type.Name.ToLowerInvariant()} found with id {id}.");
            }

            var copyOfNewObj = newObj;
            if (copyOfNewObj is ITimeStampable tsCopyOfNewObj)
            {
                tsCopyOfNewObj.Timestamps = new Data.Models.Timestamps()
                {
                    Created = ((ITimeStampable)oldObj).Timestamps.Created,
                    Updated = DateTime.UtcNow
                };
            }

            return GetCollection(newObj).ReplaceOne((o => ((IIdentifiable)o!).Id == id), newObj).IsAcknowledged;
        }

        public async Task<(Guid?, T)> InsertAsync<T>(T newObj)
        {
            var prepared = PrepareForInsertion(newObj);
            var type = newObj.GetType();
            await GetCollection(newObj).InsertOneAsync(prepared);

            return prepared is IIdentifiable
                ? (((IIdentifiable)prepared).Id, prepared)
                : (null, prepared);
        }

        public async Task<DeleteResult> DeleteAsync<T>(Guid id)
        {
            var type = typeof(T);
            if (!type.GetInterfaces().Contains(typeof(IIdentifiable)))
            {
                throw new ArgumentException("Can't delete a non-identifiable object with this method.");
            }

            var coll = GetCollection<T>();
            return await coll.DeleteOneAsync<T>((o => ((IIdentifiable)o).Id == id));
        }

        public T[] List<T>(int? skip, int? maxItems)
        {
            return GetCollection<T>()
                .AsQueryable()
                .Skip(skip.GetValueOrDefault())
                .Take(maxItems.GetValueOrDefault())
                .ToArray();
        }

        public async Task<long> Count<T>()
        {
            return await GetCollection<T>().CountDocumentsAsync((x => true));
        }

        private T PrepareForInsertion<T>(T newObject)
        {
            if (newObject == null)
            {
                throw new ArgumentNullException("Can't prepare a null object for insertion.");
            }
            var type = newObject.GetType();
            var copy = System.Text.Json.JsonSerializer.Deserialize(System.Text.Json.JsonSerializer.Serialize(newObject), type);
            if (type.GetInterfaces().Contains(typeof(IIdentifiable)))
            {
                ((IIdentifiable)copy).Id = Guid.NewGuid();
            }
            if (type.GetInterfaces().Contains(typeof(ITimeStampable)))
            {
                ((ITimeStampable)copy).Timestamps = new Data.Models.Timestamps() { Created = DateTime.UtcNow };
            }

            if (copy == null)
            {
                throw new Exception("Somehow the preparation of object for insertion resulted in a null object.");
            }

            return (T)copy;
        }

        private IMongoCollection<T> GetCollection<T>(T obj)
        {
            var db = GetDatabase();
            var type = obj.GetType();
            var collectionName = $"{type.Name.ToLowerInvariant()}s";

            return collectionName switch
            {
                "persons" => db.GetCollection<T>("people"),
                _ => db.GetCollection<T>(collectionName)
            };
        }

        private IMongoCollection<T> GetCollection<T>()
        {
            var db = GetDatabase();
            var type = typeof(T);
            var collectionName = $"{type.Name.ToLowerInvariant()}s";

            return collectionName switch
            {
                "persons" => db.GetCollection<T>("people"),
                _ => db.GetCollection<T>(collectionName)
            };
        }

        public IMongoDatabase GetDatabase() => MongoClient.GetDatabase(dbName);
    }
}