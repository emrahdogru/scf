using Scf.Controllers;
using Scf.Domain.Services;
using Scf.Domain;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scf.Models.Forms;
using MongoDB.Bson;
using Scf.Domain.TenantModels;
using Scf.Models.Filters;
using System.Text;
using Scf.Models;

namespace Scf.IntegrationTest
{
    [Collection(nameof(SignletonServiceFixture))]
    public class GroupControllerTest
    {
        readonly SignletonServiceFixture Fixture;

        public GroupControllerTest(SignletonServiceFixture fixture)
        {
            Fixture = fixture;
        }

        internal static async Task<Group> GenerateRandomGroup(DomainContext domainContext, ITenant tenant, Group? parent = null, Employee? manager = null)
        {
            var group = domainContext.Groups.Create(tenant);
            group.Id = ObjectId.GenerateNewId();
            group.Names = new MultilanguageText(Guid.NewGuid().ToString().Split("-")[0], Guid.NewGuid().ToString().Split("-")[0]);
            group.Parent = parent;
            group.Manager = manager;
            await domainContext.Groups.SaveAsync(group);
            return group;
        }

        private GroupController CreateController()
        {
            using var scope = this.Fixture.ServiceProvider.CreateScope();
            var helper = new Helper(scope);

            helper.SetAuthenticationToken(Fixture.AbsoluteUserToken.Key);
            helper.SetTenant(Fixture.AbsoluteTenant.Id);

            return ActivatorUtilities.GetServiceOrCreateInstance<GroupController>(scope.ServiceProvider);
        }

        [Fact]
        public async Task Put_Success()
        {
            var form = new GroupForm()
            {
                Id = ObjectId.GenerateNewId(),
                Name = new MultilanguageText("Name", "Name"),
                ManagerId = null,
                ParentId = null
            };

            var result = await CreateController().PutAsync(form);

            Assert.Equal(form.Id, result.Id);
            Assert.Equal(form.Name, result.Name);
            Assert.Equal(form.ManagerId, result.ManagerId);
            Assert.Equal(form.ParentId, result.ParentId);
        }

        [Fact]
        public async Task Put_Fail_InvalidManager()
        {
            var managerId = ObjectId.GenerateNewId();

            var form = new GroupForm()
            {
                Id = ObjectId.GenerateNewId(),
                Name = new MultilanguageText("Name", "Name"),
                ManagerId = managerId,
                ParentId = null
            };


            try
            {
                var result = await CreateController().PutAsync(form);
                Assert.Fail("Olmayan yönetici gönderdik. Hata vermesi gerekirdi.");
            }
            catch(EntityNotFountException ex)
            {
                Assert.Equal(nameof(Employee), ex.EntitySetName);
                Assert.Equal(managerId.ToString(), ex.EntityId);
            }
        }

        [Fact]
        public async Task Put_Fail_InvalidParent()
        {
            var parentId = ObjectId.GenerateNewId();

            var form = new GroupForm()
            {
                Id = ObjectId.GenerateNewId(),
                Name = new MultilanguageText("Name", "Name"),
                ManagerId = null,
                ParentId = parentId
            };

            try
            {
                var result = await CreateController().PutAsync(form);
                Assert.Fail("Olmayan parent gönderdik. Hata vermesi gerekirdi.");
            }
            catch (EntityNotFountException ex)
            {
                Assert.Equal(nameof(Group), ex.EntitySetName);
                Assert.Equal(parentId.ToString(), ex.EntityId);
            }
        }

        [Fact]
        public async Task Put_Fail_CircularParent()
        {
            var domainContext = this.Fixture.ServiceProvider.GetService<DomainContext>();
            var tenant = this.Fixture.AbsoluteTenant;

            List<Group> groups = new();

            for(int k = 0; k < 5; k++)
            {
                var group = await GenerateRandomGroup(domainContext, tenant, k == 0 ? null : groups[k - 1]);
                groups.Add(group);
            }


            var form = new GroupForm()
            {
                Id = groups[0].Id,
                Name = groups[0].Names,
                ManagerId = groups[0].Manager?.Id,
                ParentId = groups.First(x => x.Parent == null).Id
            };

            try
            {
                var result = await CreateController().PutAsync(form);
            }
            catch(FluentValidation.ValidationException ex)
            {
                var l = this.Fixture.ServiceProvider.GetService<ILanguageService>();
                Assert.Contains(ex.Errors, x => x.ErrorMessage == l.Get(x => x.TheMainGroupCannotBeOneOfTheSubgroups));
            }

        }

        [Fact]
        public async Task Get_Success()
        {
            var dc = this.Fixture.ServiceProvider.GetService<DomainContext>();
            var entity = dc.Groups.Create(Fixture.AbsoluteTenant);
            entity.Id = ObjectId.GenerateNewId();
            entity.Names = new MultilanguageText("Ana Grup", "Ana Grup");
            dc.Groups.SaveAsync(entity).Wait();

            var result = await CreateController().GetAsync(entity.Id);

            Assert.NotNull(result);
            Assert.Equal(result.Id, entity.Id);
            Assert.Equal(result.Name, entity.Names);
            Assert.Equal(result.ManagerId, entity.Manager?.Id);
            Assert.Equal(result.ParentId, entity.Parent?.Id);
        }

        [Fact]
        public async Task Get_Fail_NotExist()
        {
            var id = ObjectId.GenerateNewId();

            try
            {
                var result = await CreateController().GetAsync(id);
            }
            catch(EntityNotFountException ex)
            {
                Assert.Equal(nameof(Group), ex.EntitySetName);
                Assert.Equal(id.ToString(), ex.EntityId);
            }
        }

        public static IEnumerable<object[]> filters = new List<object[]>()
        {
            new object[]{ new GroupFilter(){ Search = "a" } },
            new object[]{ new GroupFilter(){ Page = 1, PageSize = 1, Sorts = new FilterSort[] { new("Names.tr", true) } } },
            new object[]{ new GroupFilter(){ PageSize = 1, Sorts = new FilterSort[] { new("Names.tr") } } },
        };

        [Theory]
        [MemberData(nameof(filters))]
        public async Task List_Success(GroupFilter filter)
        {
            var domainContext = this.Fixture.ServiceProvider.GetService<DomainContext>();
            var tenant = this.Fixture.AbsoluteTenant;

            List<Group> groups = new();

            for (int k = 0; k < 10; k++)
            {
                var group = await GenerateRandomGroup(domainContext, tenant);
                groups.Add(group);
            }

            var result = CreateController().List(filter);

            if(filter.PageSize.HasValue)
                Assert.Equal(filter.PageSize, result.Result.Length);
        }
    }
}
