using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Scf.Domain
{
    /// <summary>
    /// Hesap için özelleştirilmiş dil tanımları
    /// </summary>
    public class TenantLanguageResource
    {

        [BsonId]
        public ObjectId TenantId { get; set; }

        public Dictionary<string, LanguageResources.L>? Resource { get; set; }

        public static IMongoCollection<TenantLanguageResource> GetMongoCollection(IMongoDatabase database)
        {
            return database.GetCollection<TenantLanguageResource>("TenantLanguageResource");
        }

        public static IQueryable<TenantLanguageResource> GetAll(IMongoDatabase database)
        {
            return GetMongoCollection(database).AsQueryable();
        }

        public void Save(IMongoDatabase database)
        {
            var replaceOptions = new ReplaceOptions() { IsUpsert = true };
            try
            {
                GetMongoCollection(database).ReplaceOneAsync(x => x.TenantId == TenantId, this, replaceOptions);
            }
            catch (MongoWriteException ex)
            {
                //A bulk write operation resulted in one or more errors.
                // E11000 duplicate key error index: bastapp.User.$Email dup key: { : "emrahdogru@gmail.com", : ObjectId('57d830d40f42fa2bd8ecad4a') }
                if (ex.Message.Contains("E11000"))
                {
                    throw new Exception("DuplicateKeyError", ex);
                }
                else
                {
                    throw;
                }
            }
        }

        public static TenantLanguageResource? Get(IMongoDatabase database, ObjectId tenantId)
        {
            return GetAll(database).FirstOrDefault(x => x.TenantId == tenantId);
        }
    }
}
