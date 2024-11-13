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
    public class PositionControllerTest
    {
        readonly SignletonServiceFixture Fixture;

        public PositionControllerTest(SignletonServiceFixture fixture)
        {
            Fixture = fixture;
        }

        private PositionController CreateController()
        {
            using var scope = this.Fixture.ServiceProvider.CreateScope();
            var helper = new Helper(scope);

            helper.SetAuthenticationToken(Fixture.AbsoluteUserToken.Key);
            helper.SetTenant(Fixture.AbsoluteTenant.Id);

            return ActivatorUtilities.GetServiceOrCreateInstance<PositionController>(scope.ServiceProvider);
        }

        public static async Task<Position> GenerateRandomPosition(DomainContext domainContext, Tenant tenant)
        {
            var position = domainContext.Positions.Create(tenant);
            position.Id = ObjectId.GenerateNewId();
            position.Names = new MultilanguageText(Guid.NewGuid().ToString().Split("-")[0], Guid.NewGuid().ToString().Split("-")[0]);
            await domainContext.Positions.SaveAsync(position);
            return position;
        }

        [Fact]
        public async Task Put_Success()
        {
            var form = new PositionForm()
            {
                Id = ObjectId.GenerateNewId(),
                Name = new MultilanguageText("Name", "Name")
            };

            var result = await CreateController().PutAsync(form);

            Assert.NotNull(result);
            Assert.Equal(form.Id, result.Id);
            Assert.Equal(form.Name, result.Name);
        }

        [Fact]
        public async Task Get_Success()
        {
            using var scope = this.Fixture.ServiceProvider.CreateScope();
            var domainContext = scope.ServiceProvider.GetService<DomainContext>();
            var tenant = Fixture.AbsoluteTenant;
            var position = await GenerateRandomPosition(domainContext, tenant);

            var form = await CreateController().GetAsync(position.Id);

            Assert.NotNull(form);
            Assert.Equal(position.Id, form.Id);
            Assert.Equal(position.Names, form.Name);
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
            catch (EntityNotFountException ex) {
                Assert.Equal(id.ToString(), ex.EntityId);
                Assert.Equal(nameof(Position), ex.EntitySetName);
            }
        }



        public static readonly IEnumerable<object[]> filters = new List<object[]>()
        {
            new object[]{ new FilterSearch<Position>(){ Search = "a" } },
            new object[]{ new FilterSearch<Position>(){ Page = 1, PageSize = 1, Sorts = new FilterSort[] { new("Names.tr", true) } } },
            new object[]{ new FilterSearch<Position>(){ PageSize = 1, Sorts = new FilterSort[] { new("Names.tr") } } },
        };

        [Theory]
        [MemberData(nameof(filters))]
        public async Task List_Success(FilterSearch<Position> filter)
        {
            var domainContext = this.Fixture.ServiceProvider.GetService<DomainContext>();
            var tenant = this.Fixture.AbsoluteTenant;

            var positions = new List<Position>();

            for (int k = 0; k < 10; k++)
            {
                var position = await GenerateRandomPosition(domainContext, tenant);
                positions.Add(position);
            }

            var result = CreateController().List(filter);

            if (filter.PageSize.HasValue)
                Assert.Equal(filter.PageSize, result.Result.Length);
        }
    }
}
