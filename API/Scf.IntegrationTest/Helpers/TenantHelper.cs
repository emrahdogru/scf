using Scf.Domain;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.IntegrationTest.Helpers
{
    public class TenantHelper
    {
        private readonly IServiceScope scope;

        public static readonly ObjectId AbsoluteTenantId = ObjectId.Parse("63f5bcbb7720a895dcd3b817");

        public TenantHelper(IServiceScope scope)
        {
            this.scope = scope;
        }

        public async Task<Tenant> GetAbsoluteTenant()
        {
            var db = scope.ServiceProvider.GetService<DomainContext>();

            var tenant = db.Tenants.Create();
            tenant.Id = AbsoluteTenantId;
            tenant.Title = "Test Hesabı";
            tenant.Code = "test";
            tenant.DefaultLanguage = Languages.Turkish;
            await db.Tenants.SaveAsync(tenant);

            return tenant;
        }
    }
}
