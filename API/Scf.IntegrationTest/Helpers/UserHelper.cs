using Scf.Domain;
using Scf.Domain.Services;
using Scf.IntegrationTest.FakeServices;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.IntegrationTest.Helpers
{
    public class UserHelper
    {
        public static readonly ObjectId AbsoluteUserId = ObjectId.Parse("63f5bcbb7720a895dcd3b817");
        public const string AbsoluteUserEmail = "testuser@example.com";
        public const string AbsoluteUserPassword = "Test-1234";

        private readonly IServiceScope scope;
        readonly DomainContext domainContext;

        public UserHelper(IServiceScope scope) { 
            this.scope = scope;
            this.domainContext = scope.ServiceProvider.GetService<DomainContext>();
        }

        public User CreateRandomUser(out string password)
        {
            string email = $"{Guid.NewGuid():N}@example.com";

            var user = domainContext.Users.Create();
            user.Email = email;
            user.FirstName = Guid.NewGuid().ToString().Split("-")[0];
            user.LastName = Guid.NewGuid().ToString().Split("-")[1];
            user.IsActive = true;
            user.Language = Languages.Turkish;

            password = Guid.NewGuid().ToString();
            user.SetPassword(password);

            return user;
        }

        public async Task<Token> GenerateToken(User user)
        {
            var tokenService = scope.ServiceProvider.GetService<ITokenService>();
            var token = await tokenService.Generate(user, Token.TokenSource.Web, Token.TokenKind.Maintenance);
            return token;
        }


    }
}
