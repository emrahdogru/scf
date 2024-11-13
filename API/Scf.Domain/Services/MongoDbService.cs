using Scf.Utility;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Scf.Domain.Services
{
    public class MongoDbService : IMongoDbService
    {
        private readonly Configuration _configuration;
        private readonly IMongoClientService _mongoClientService;

        private IMongoDatabase? _database;

        public MongoDbService(Configuration configuration, IMongoClientService mongoClientService)
        {
            _configuration = configuration;
            _mongoClientService = mongoClientService;
        }

        public MongoDbService(IOptions<Configuration> configuration, IMongoClientService clientService)
            :this(configuration.Value, clientService)
        {
            
        }

        public IMongoDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    string? databaseName = _configuration.DatabaseName;

                    if (string.IsNullOrWhiteSpace(databaseName))
                        throw new NullReferenceException("Konfigürasyon dosyasında 'DatabaseName' boş olamaz.");

                    _database = _mongoClientService.Client.GetDatabase(databaseName);
                }

                return _database;
            }
        }
    }
}
