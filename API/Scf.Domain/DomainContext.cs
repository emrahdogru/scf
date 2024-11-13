using Scf.Database;
using Scf.Domain.EntitySets;
using Scf.Domain.Services;
using Scf.Domain.TenantModels;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Scf.Domain
{
    public class DomainContext : EntityContext
    {
        private readonly ILanguageService languageService;

        public DomainContext(IMongoDbService service, IOptions<AppSettings> appSettings, ILanguageService languageService) : base(service.Database)
        {
            this.languageService = languageService;
            this.AppSettings = appSettings.Value;
            Initialize();
        }

        public DomainContext(IMongoDatabase database, AppSettings appSettings, ILanguageService languageService) : base(database)
        {
            this.languageService = languageService;
            this.AppSettings = appSettings;
            Initialize();
        }



        private void Initialize()
        {
            this.Notifications = new EntitySet<NotificationQueue>(this, "Notification", 1)
            {
                Indexes = new EntitySetIndex<NotificationQueue>[]
                {
                    new (x => x.Ascending(f => f.PersonId).Ascending(f => f.PersonType))
                }
            };

            this.PasswordResetRequests = new PasswordResetRequestEntitySet(this, "PasswordResetRequest");

            this.Users = new UserEntitySet(this, "User", 1)
            {
                Indexes = new EntitySetIndex<User>[]
                {
                    new (x => x.Ascending(f => f.TenantIds)),
                    new (
                        x => x.Ascending(f => f.Email),
                        new (){
                            Collation = new Collation("en_US"),
                            Unique = true
                        }
                    )
                }
            };

            this.Tenants = new TenantEntitySet(this, "Tenant", 1)
            {
                Indexes = new EntitySetIndex<Tenant>[]
                {
                    new (
                        x => x.Ascending(f => f.Code),
                        new (){
                            Collation = new Collation("en_US"),
                            Unique = true
                        }
                    )
                }
            };

            this.Tokens = new EntitySet<Token>(this, "Token", 1)
            {
                Indexes = new EntitySetIndex<Token>[]
                {
                    new (
                        x => x.Ascending(f => f.Key),
                        new (){
                            Collation = new Collation("en_US")
                        }
                    )
                }
            };

            this.Employees = new EntitySetWithTenant<Employee>(this, "Employee", 4)
            {
                Indexes = new EntitySetIndex<Employee>[]
                {
                    new(x => x.Ascending(f => f.TenantId)),
                    new (
                        x => x.Ascending(f => f.TenantId).Ascending(f => f.Email),
                        new (){
                            Collation = new Collation("en_US"),
                            Unique = true
                        }
                    ),
                    new(
                        x => x.Ascending(f => f.TenantId).Ascending(f => f.UserId),
                        new (){ 
                            Unique = true,
                            PartialFilterExpression = new FilterDefinitionBuilder<Employee>().Type(x => x.UserId, BsonType.ObjectId)
                        }
                    )
                }
            };

            this.Groups = new EntitySetWithTenant<Group>(this, "Group", 1)
            { 
                Indexes = new EntitySetIndex<Group>[]
                { 
                    new (x => x.Ascending(f => f.TenantId))
                }
            };

            this.Positions = new EntitySetWithTenant<Position>(this, "Position", 1)
            {
                Indexes = new EntitySetIndex<Position>[]
                {
                    new (x => x.Ascending(f => f.TenantId))
                }
            };

            this.EmployeeTitles = new EntitySetWithTenant<EmployeeTitle>(this, "EmployeeTitle", 1)
            {
                Indexes = new EntitySetIndex<EmployeeTitle>[]
                {
                    new (x => x.Ascending(f => f.TenantId))
                }
            };

            this.Items = new EntitySetWithTenant<Item>(this, "Item", 1);
            this.Accounts = new EntitySetWithTenant<Account>(this, "Account", 1);
            this.Documents = new EntitySetWithTenant<Document>(this, "Document", 1);

        }

        internal new IMongoDatabase Database
        {
            get
            {
                return base.Database;
            }
        }

        public ILanguageService LanguageService { get => languageService; }

        public AppSettings AppSettings { get; private set; }
        public PasswordResetRequestEntitySet PasswordResetRequests { get; private set; } = null!;
        public EntitySet<NotificationQueue> Notifications { get; private set; } = null!;
        public UserEntitySet Users { get; private set; } = null!;
        public TenantEntitySet Tenants { get; private set; } = null!;
        public EntitySet<Token> Tokens { get; private set; } = null!;
        public EntitySetWithTenant<Employee> Employees { get; private set; } = null!;
        public EntitySetWithTenant<Group> Groups { get; private set; } = null!;
        public EntitySetWithTenant<Position> Positions { get; private set; } = null!;
        public EntitySetWithTenant<EmployeeTitle> EmployeeTitles { get; private set; } = null!;

        /// <summary>
        /// Stok ve hizmetler
        /// </summary>
        public EntitySetWithTenant<Item> Items { get; private set; } = null!;
        public EntitySetWithTenant<Account> Accounts { get; private set; } = null!;
        public EntitySetWithTenant<Document> Documents { get; private set; } = null!;

    }
}
