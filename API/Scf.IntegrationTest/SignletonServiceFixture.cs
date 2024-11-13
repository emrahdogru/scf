using Scf.Domain.Services;
using Scf.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scf.IntegrationTest.FakeServices;
using Microsoft.Extensions.DependencyInjection;
using Scf.Utility;
using Scf.IntegrationTest.Helpers;
using MongoDB.Bson;

namespace Scf.IntegrationTest
{
    public class SignletonServiceFixture : IDisposable
    {
        public SignletonServiceFixture() {
            this.Configuration = new Utility.Configuration()
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = $"Scf_Test_{Guid.NewGuid():N}"
            };

            this.AppSettings = new AppSettings()
            {
                TokenAbsoluteTimeout = 9999,
                UIDomain = "https://ui.scf.com"
            };

            var builder = new ServiceCollection();

            builder.Configure<Configuration>(x => {
                x.DatabaseName = this.Configuration.DatabaseName;
                x.ConnectionString = this.Configuration.ConnectionString;
            });

            builder.Configure<AppSettings>(x =>
            {
                x.TokenAbsoluteTimeout = this.AppSettings.TokenAbsoluteTimeout;
                x.UIDomain = this.AppSettings.UIDomain;
            });

            builder.AddSingleton<IMongoClientService, MongoClientService>();
            builder.AddSingleton<IMongoDbService, MongoDbService>();

            builder.AddScoped<IHttpContextService, FakeHttpContextService>();
            
            builder.AddScoped<DomainContext>();
            builder.AddScoped<ITokenService, TokenService>();
            builder.AddScoped<ISessionService, SessionService>();
            builder.AddScoped<ILoggerService, LoggerService>();
            builder.AddScoped<ILanguageService, LanguageService>();

            builder.AddScoped<UserService>();
            builder.AddScoped<EmployeeService>();
            builder.AddScoped<PositionService>();
            builder.AddScoped<NotificationService>();
            builder.AddScoped<GroupService>();
            builder.AddScoped<EmployeeTitleService>();
            builder.AddScoped<PremiumCycleService>();
            builder.AddScoped<PremiumFileService>();

            this.ServiceProvider = builder.BuildServiceProvider();

            InitializeData().Wait();
        }

        public ServiceProvider ServiceProvider { get;}

        public Utility.Configuration Configuration { get; }
        public AppSettings AppSettings { get; }

        public User AbsoluteUser { get; private set; }
        public Token AbsoluteUserToken { get; private set; }

        public Tenant AbsoluteTenant { get; private set; }

        private async Task InitializeData()
        {
            var domainContext = this.ServiceProvider.GetService<DomainContext>();

            await domainContext.InitializeDatabase();

            var tenant = domainContext.Tenants.Create();
            tenant.Id = TenantHelper.AbsoluteTenantId;
            tenant.Title = "Test Hesabı";
            tenant.Code = "test";
            tenant.DefaultLanguage = Languages.Turkish;
            await domainContext.Tenants.SaveAsync(tenant);
            this.AbsoluteTenant = tenant;

            var user = domainContext.Users.Create();
            user.Id = UserHelper.AbsoluteUserId;
            user.Email = UserHelper.AbsoluteUserEmail;
            user.FirstName = "Test";
            user.LastName = "User";
            user.IsActive = true;
            user.Language = Languages.Turkish;
            user.AddToTenant(tenant);
            user.SetPassword(UserHelper.AbsoluteUserPassword);
            await domainContext.Users.SaveAsync(user);
            this.AbsoluteUser = user;

            var tokenService = this.ServiceProvider.GetService<ITokenService>();
            var token = tokenService.Generate(user, Token.TokenSource.Web, Token.TokenKind.Maintenance).Result;
            await domainContext.Tokens.SaveAsync(token);
            this.AbsoluteUserToken = token;



            //var group2 = domainContext.Groups.Create(tenant);
            //group2.Id = ObjectId.GenerateNewId();
            //group2.Name = "Alt Grup";
            //group2.Parent = group;
            //domainContext.Groups.SaveAsync(group2).Wait();

        }

        void IDisposable.Dispose()
        {
            this.ServiceProvider.GetService<IMongoClientService>().Client.DropDatabase(this.Configuration.DatabaseName);
        }
    }

    [CollectionDefinition(nameof(SignletonServiceFixture))]
    public class SignletonServiceCollection : ICollectionFixture<SignletonServiceFixture>
    { }
}
