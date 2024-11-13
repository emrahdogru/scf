using Scf.Controllers;
using Scf.Domain;
using Scf.Domain.TenantModels;
using Scf.Models;
using Scf.Models.Filters;
using Scf.Models.Forms;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.IntegrationTest
{
    [Collection(nameof(SignletonServiceFixture))]
    public class EmployeeTitleControllerTest
    {
        readonly SignletonServiceFixture Fixture;

        public EmployeeTitleControllerTest(SignletonServiceFixture fixture)
        {
            Fixture = fixture;
        }

        private EmployeeTitleController CreateController()
        {
            using var scope = this.Fixture.ServiceProvider.CreateScope();
            var helper = new Helper(scope);

            helper.SetAuthenticationToken(Fixture.AbsoluteUserToken.Key);
            helper.SetTenant(Fixture.AbsoluteTenant.Id);

            return ActivatorUtilities.GetServiceOrCreateInstance<EmployeeTitleController>(scope.ServiceProvider);
        }

        internal static async Task<EmployeeTitle> GenerateRandomEmployeeTitle(DomainContext domainContext, Tenant tenant)
        {
            var title = domainContext.EmployeeTitles.Create(tenant);
            title.Id = ObjectId.GenerateNewId();
            title.Names = new MultilanguageText(Guid.NewGuid().ToString().Split("-")[0], Guid.NewGuid().ToString().Split("-")[0]);
            await domainContext.EmployeeTitles.SaveAsync(title);
            return title;
        }

        [Fact]
        public async Task Put_Success()
        {
            var form = new EmployeeTitleForm()
            {
                Id = ObjectId.GenerateNewId(),
                Names = new MultilanguageText(Guid.NewGuid().ToString().Split("-")[0], Guid.NewGuid().ToString().Split("-")[0])
            };

            var result = await CreateController().PutAsync(form);

            Assert.NotNull(result);
            Assert.Equal(form.Id, result.Id);
            Assert.Equal(form.Names, result.Names);
        }

        [Fact]
        public async Task Get_Success()
        {
            using var scope = this.Fixture.ServiceProvider.CreateScope();
            var domainContext = scope.ServiceProvider.GetService<DomainContext>();
            var tenant = Fixture.AbsoluteTenant;
            var EmployeeTitle = await GenerateRandomEmployeeTitle(domainContext, tenant);

            var form = await CreateController().GetAsync(EmployeeTitle.Id);

            Assert.NotNull(form);
            Assert.Equal(EmployeeTitle.Id, form.Id);
            Assert.Equal(EmployeeTitle.Names, form.Names);
        }

        [Fact]
        public async Task Get_Fail_NotExist()
        {
            var id = ObjectId.GenerateNewId();

            try
            {
                var form = await CreateController().GetAsync(id);
                Assert.Fail("Olmayan bir kayıt için hata vermesi lazımdı.");
            }
            catch (EntityNotFountException ex)
            {
                Assert.Equal(id.ToString(), ex.EntityId);
                Assert.Equal(nameof(EmployeeTitle), ex.EntitySetName);
            }
        }



        public static IEnumerable<object[]> filters = new List<object[]>()
        {
            new object[]{ new FilterSearch<EmployeeTitle>(){ Search = "a" } },
            new object[]{ new FilterSearch<EmployeeTitle>(){ Page = 1, PageSize = 1, Sorts = new FilterSort[] { new("Names.tr", true) } } },
            new object[]{ new FilterSearch<EmployeeTitle>(){ PageSize = 1, Sorts = new FilterSort[] { new("Names.tr") } } },
        };

        [Theory]
        [MemberData(nameof(filters))]
        public async Task List_Success(FilterSearch<EmployeeTitle> filter)
        {
            var domainContext = this.Fixture.ServiceProvider.GetService<DomainContext>();
            var tenant = this.Fixture.AbsoluteTenant;

            List<EmployeeTitle> EmployeeTitles = new();

            for (int k = 0; k < 10; k++)
            {
                var EmployeeTitle = await GenerateRandomEmployeeTitle(domainContext, tenant);
                EmployeeTitles.Add(EmployeeTitle);
            }

            var result = CreateController().List(filter);

            if (filter.PageSize.HasValue)
                Assert.Equal(filter.PageSize, result.Result.Length);
        }
    }
}
