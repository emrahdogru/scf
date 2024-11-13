using Scf.Controllers;
using Scf.Domain;
using Scf.Domain.Services;
using Scf.Domain.TenantModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.IntegrationTest
{
    [Collection(nameof(SignletonServiceFixture))]
    public class TenantControllerTest
    {
        readonly SignletonServiceFixture Fixture;

        public TenantControllerTest(SignletonServiceFixture fixture)
        {
            Fixture = fixture;
        }

        private TenantController CreateController()
        {
            using var scope = this.Fixture.ServiceProvider.CreateScope();
            var helper = new Helper(scope);

            helper.SetAuthenticationToken(Fixture.AbsoluteUserToken.Key);
            helper.SetTenant(Fixture.AbsoluteTenant.Id);

            return ActivatorUtilities.GetServiceOrCreateInstance<TenantController>(scope.ServiceProvider);
        }

        internal static async Task<Tenant> GenerateRandomTenant(DomainContext domainContext)
        {
            var tenant = domainContext.Tenants.Create();
            tenant.Id = ObjectId.GenerateNewId();
            tenant.Title = Guid.NewGuid().ToString().Split("-")[0];
            tenant.Code = Guid.NewGuid().ToString().Split("-")[1];
            await domainContext.Tenants.SaveAsync(tenant);
            return tenant;
        }

        [Fact]
        public async Task Detail_Success()
        {
            using var scope = Fixture.ServiceProvider.CreateScope();
            var helper = new Helper(scope);

            helper.SetAuthenticationToken(Fixture.AbsoluteUserToken.Key);


            var controller = CreateController();

            var tenant = await controller.Detail(Fixture.AbsoluteTenant.Id);

            Assert.NotNull(tenant);
        }

        [Fact]
        public async Task Detail_Fail_NotFound()
        {
            using var scope = Fixture.ServiceProvider.CreateScope();
            var helper = new Helper(scope);
            helper.SetAuthenticationToken(Fixture.AbsoluteUserToken.Key);


            var controller = CreateController();

            ObjectId tenantId = ObjectId.GenerateNewId();

            try
            {
                var tenant = await controller.Detail(tenantId);
                Assert.Fail("EntityNotFound bekliyorduk.");
            }
            catch(EntityNotFountException ex)
            {
                var l = scope.ServiceProvider.GetService<ILanguageService>();
                Assert.Equal(nameof(Tenant), ex.EntitySetName);
                Assert.Equal(ex.EntityId, tenantId.ToString());
            }
        }

        [Fact]
        public async Task Detail_Fail_UserUnauthorizedOnThisTenant()
        {
            using var scope = Fixture.ServiceProvider.CreateScope();
            var helper = new Helper(scope);
            helper.SetAuthenticationToken(Fixture.AbsoluteUserToken.Key);
            var domainContext = scope.ServiceProvider.GetService<DomainContext>();

            var tenant = await GenerateRandomTenant(domainContext);

            var controller = CreateController();

            try
            {
                var result = await controller.Detail(tenant.Id);
                Assert.Fail("UserAuthorizationException bekliyorduk.");
            }
            catch (UserAuthorizationException)
            {

            }
        }
    }
}
