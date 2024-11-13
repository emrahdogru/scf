using Scf.Database;
using Scf.Utility;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Scf.Domain.Services
{
    public class MongoClientService : IMongoClientService
    {
        readonly Configuration _configuration;
        private IMongoClient? client = null;

        public MongoClientService(IOptions<Configuration> configuration)
        {
            _configuration = configuration.Value;
            RegisterSerializers();
        }

        public MongoClientService(Configuration configuration)
        {
            _configuration = configuration;
            RegisterSerializers();
        }

        private static void RegisterSerializers()
        {
            try
            {
                _ = MongoDB.Bson.Serialization.BsonSerializer.TryRegisterSerializer<DateTime>(new MongoDateTimeSerializer());
            }
            catch (MongoDB.Bson.BsonSerializationException)
            {
                //throw new Exception("MongoDateTimeSerializer, register edilemedi.", ex);
            }
        }

        public IMongoClient Client
        {
            get
            {
                if (client == null)
                {
                    string? connStr = _configuration.ConnectionString;

                    if (string.IsNullOrWhiteSpace(connStr))
                        throw new ArgumentException("ConnectionString tanımlı değil.");

                    client = new MongoClient(connStr);
                }

                return client;
            }
        }


    }
}
