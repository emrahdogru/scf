using Scf.Controllers;
using Scf.Domain;
using Scf.Domain.TenantModels;
using Scf.Domain.TenantModels.Premium;
using Scf.Models.Forms.Premium;
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
    public class PremiumCycleControllerTest
    {
        readonly SignletonServiceFixture Fixture;

        public PremiumCycleControllerTest(SignletonServiceFixture fixture)
        {
            Fixture = fixture;
        }

        private PremiumCycleController CreateController()
        {
            using var scope = this.Fixture.ServiceProvider.CreateScope();
            var helper = new Helper(scope);

            helper.SetAuthenticationToken(Fixture.AbsoluteUserToken.Key);
            helper.SetTenant(Fixture.AbsoluteTenant.Id);

            return ActivatorUtilities.GetServiceOrCreateInstance<PremiumCycleController>(scope.ServiceProvider);
        }

        public static async Task<PremiumCycle> CreateRandomPremiumCycle(DomainContext context, Tenant tenant)
        {
            var employee = await EmployeeControllerTest.GenerateRandomEmployee(context, tenant);

            var c = new PremiumCycle(context, tenant);
            c.StartDate = DateTime.UtcNow;
            c.EndDate = DateTime.UtcNow.AddMonths(1);
            c.Names = new MultilanguageText(Guid.NewGuid().ToString().Split("-")[0], Guid.NewGuid().ToString().Split("-")[0]);
            c.DeviationRate = 5;
            c.Managers = new Employee[] { employee };
            c.TotalBudget = 500000;
            c.BaseAmountFormula = "";

            await context.PremiumCycles.SaveAsync(c);
            return c;
        }

        [Fact]
        public async Task Put_Success()
        {
            var form = new PremiumCycleForm();
            form.StartDate = DateTime.Now;
            form.EndDate = DateTime.Now.AddMonths(1);
            form.Names.tr = "Prim dönemi";
            form.Names.en = "Premium cycle";
            form.IsActive = true;
            form.DeviationRate = 5;
            form.BaseAmountFormula = "";

            var controller = CreateController();
            var result = await controller.PutAsync(form);

            Assert.NotNull(result);
            Assert.Equal(result.StartDate, form.StartDate);
            Assert.Equal(result.EndDate, form.EndDate);
            Assert.Equal(result.Names.tr, form.Names.tr);
            Assert.Equal(result.IsActive, form.IsActive);
            Assert.Equal(result.DeviationRate, form.DeviationRate);
            Assert.Equal(result.BaseAmountFormula, form.BaseAmountFormula);
        }

        [Fact]
        public async Task Get_Success()
        {
            using var scope = this.Fixture.ServiceProvider.CreateScope();
            var domainContext = scope.ServiceProvider.GetService<DomainContext>();
            var tenant = Fixture.AbsoluteTenant;

            var cycle = await CreateRandomPremiumCycle(domainContext, tenant);

            var controller = CreateController();
            var result = await controller.GetAsync(cycle.Id);

            Assert.NotNull(result);
            Assert.Equal(result.StartDate.ToString(), cycle.StartDate.ToString());
            Assert.Equal(result.EndDate.ToString(), cycle.EndDate.ToString());
            Assert.Equal(result.Names.tr, cycle.Names.tr);
            Assert.Equal(result.IsActive, cycle.IsActive);
            Assert.Equal(result.DeviationRate, cycle.DeviationRate);
            Assert.Equal(result.BaseAmountFormula, cycle.BaseAmountFormula);
        }

        [Fact]
        public void List_Success()
        {
            var controller = CreateController();
            var result = controller.List(new Models.Filters.Premium.PremiumCycleFilter() { });
            Assert.NotNull(result);

            Assert.Equal(result.TotalRecordCount, result.Result.Length);
        }
    }
}
