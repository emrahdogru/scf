using Scf.Domain;
using Scf.Domain.Services;
using Scf.IntegrationTest.FakeServices;
using Scf.IntegrationTest.Helpers;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.IntegrationTest
{
    public class Helper
    {
        readonly IServiceScope scope;

        public Helper(IServiceScope scope) {
            this.scope = scope;
            this.UserHelper = new UserHelper(scope);
            this.TenantHelper = new TenantHelper(scope);
        }

        public UserHelper UserHelper { get; }

        public TenantHelper TenantHelper { get; }

        public void SetAuthenticationToken(string token)
        {
            var service = scope.ServiceProvider.GetService<IHttpContextService>() as FakeHttpContextService;
            service.Token = token;
        }

        public void SetAuthenticationToken(User user)
        {
            var token = this.UserHelper.GenerateToken(user).Result;
            SetAuthenticationToken(token.Key);
        }

        public void SetTenant(ObjectId? tenantId)
        {
            var service = scope.ServiceProvider.GetService<IHttpContextService>() as FakeHttpContextService;
            service.TenantId = tenantId;
        }
    }
}
