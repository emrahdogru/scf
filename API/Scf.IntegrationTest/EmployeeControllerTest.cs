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
using System.Runtime.Serialization;
using Scf.Models;

namespace Scf.IntegrationTest
{
    [Collection(nameof(SignletonServiceFixture))]
    public class EmployeeControllerTest
    {
        readonly SignletonServiceFixture Fixture;

        public EmployeeControllerTest(SignletonServiceFixture fixture)
        {
            Fixture = fixture;
        }

        internal static async Task<Employee> GenerateRandomEmployee(DomainContext domainContext, ITenant tenant, Employee? manager = null, EmployeeTitle? title = null, Position? position = null, Group[]? groups = null)
        {
            var employee = domainContext.Employees.Create(tenant);
            employee.Id = ObjectId.GenerateNewId();
            employee.FirstName = Guid.NewGuid().ToString().Split("-")[0];
            employee.LastName = Guid.NewGuid().ToString().Split('-')[1];
            employee.Email = Guid.NewGuid().ToString("N") + "@example.com";
            employee.Manager = manager;
            employee.Title = title;
            employee.Position = position;
            employee.Groups = groups ?? Array.Empty<Group>();
            await domainContext.Employees.SaveAsync(employee);
            return employee;
        }

        private EmployeeController CreateController()
        {
            using var scope = this.Fixture.ServiceProvider.CreateScope();
            var helper = new Helper(scope);

            helper.SetAuthenticationToken(Fixture.AbsoluteUserToken.Key);
            helper.SetTenant(Fixture.AbsoluteTenant.Id);

            return ActivatorUtilities.GetServiceOrCreateInstance<EmployeeController>(scope.ServiceProvider);
        }

        [Fact]
        public async Task Put_Success()
        {
            using var scope = this.Fixture.ServiceProvider.CreateScope();
            var domainContext = scope.ServiceProvider.GetService<DomainContext>();
            var tenant = this.Fixture.AbsoluteTenant;

            //var httpService = scope.ServiceProvider.GetService<IHttpContextService>() as FakeServices.FakeHttpContextService;
            //httpService.TenantId = tenant.Id;

            Group group = await GroupControllerTest.GenerateRandomGroup(domainContext, tenant);
            Position position = await PositionControllerTest.GenerateRandomPosition(domainContext, tenant);
            EmployeeTitle title = await EmployeeTitleControllerTest.GenerateRandomEmployeeTitle(domainContext, tenant);


            var form = new EmployeeForm()
            {
                Id = ObjectId.GenerateNewId(),
                FirstName = "Firstname",
                LastName = "Lastname",
                Email = "testemployee@example.com",
                ManagerId = null,
                GroupIds = new ObjectId[] { group.Id },
                PositionId = position.Id,
                TitleId = title.Id
            };

            var result = await CreateController().PutAsync(form);

            Assert.Equal(form.Id, result.Id);
            Assert.Equal(form.FirstName, result.FirstName);
            Assert.Equal(form.LastName, result.LastName);
            Assert.True(form.GroupIds.SequenceEqual(result.GroupIds));
            Assert.Equal(form.PositionId, result.PositionId);
            Assert.Equal(form.TitleId, result.TitleId);
        }


        public static IEnumerable<object[]> invalidRelatedEmployees = new List<object[]>()
        {
            new object[]{
                new EmployeeForm()
                {
                    Id = ObjectId.GenerateNewId(),
                    FirstName = "Name",
                    LastName = "Lastname",
                    Email = "unexistemployee@example.com",
                    ManagerId = ObjectId.Parse("13fa6423d86afe04a670fc9a")
                }, "Employee", "13fa6423d86afe04a670fc9a"
            },
            new object[]{
                new EmployeeForm()
                {
                    Id = ObjectId.GenerateNewId(),
                    FirstName = "Name",
                    LastName = "Lastname",
                    Email = "unexistemployee@example.com",
                    PositionId = ObjectId.Parse("13fa6423d86afe04a670fc9a")
                }, "Position", "13fa6423d86afe04a670fc9a"
            },
            new object[]{
                new EmployeeForm()
                {
                    Id = ObjectId.GenerateNewId(),
                    FirstName = "Name",
                    LastName = "Lastname",
                    Email = "unexistemployee@example.com",
                    TitleId = ObjectId.Parse("13fa6423d86afe04a670fc9a")
                }, "EmployeeTitle", "13fa6423d86afe04a670fc9a"
            },
        };

        [Theory]
        [MemberData(nameof(invalidRelatedEmployees))]
        public async Task Put_Fail_InvalidRelations(EmployeeForm form, string entityName, string id)
        {

            try
            {
                var result = await CreateController().PutAsync(form);
                Assert.Fail("Olmayan manager gönderedik. Hata vermesi gerekirdi.");
            }
            catch (EntityNotFountException ex)
            {
                Assert.Equal(entityName, ex.EntitySetName);
                Assert.Equal(id, ex.EntityId);
            }
        }


        [Fact]
        public async Task Put_Fail_CircularParent()
        {
            var domainContext = this.Fixture.ServiceProvider.GetService<DomainContext>();
            var tenant = this.Fixture.AbsoluteTenant;

            List<Employee> employees = new();

            for (int k = 0; k < 5; k++)
            {
                var employee = await GenerateRandomEmployee(domainContext, tenant, k == 0 ? null : employees[k - 1]);
                employees.Add(employee);
            }


            var form = new EmployeeForm()
            {
                Id = employees[0].Id,
                FirstName = employees[0].FirstName,
                LastName = employees[0].LastName,
                ManagerId = employees.First(x => x.Manager == null).Id
            };

            try
            {
                var result = await CreateController().PutAsync(form);
            }
            catch (FluentValidation.ValidationException ex)
            {
                var l = this.Fixture.ServiceProvider.GetService<ILanguageService>();
                Assert.Contains(ex.Errors, x => x.ErrorMessage == l.Get(x => x.TheManagerCannotBeSubordinateOfTheEmployee));
            }

        }

        [Fact]
        public async Task Get_Success()
        {
            var dc = this.Fixture.ServiceProvider.GetService<DomainContext>();
            var entity = await GenerateRandomEmployee(dc, this.Fixture.AbsoluteTenant);

            var result = await CreateController().GetAsync(entity.Id);

            Assert.NotNull(result);
            Assert.Equal(result.Id, entity.Id);
            Assert.Equal(result.FirstName, entity.FirstName);
            Assert.Equal(result.LastName, entity.LastName);
            Assert.Equal(result.ManagerId, entity.Manager?.Id);
        }

        [Fact]
        public async Task Get_Fail_NotExist()
        {
            var id = ObjectId.GenerateNewId();

            try
            {
                var result = await CreateController().GetAsync(id);
            }
            catch (EntityNotFountException ex)
            {
                Assert.Equal(nameof(Employee), ex.EntitySetName);
                Assert.Equal(id.ToString(), ex.EntityId);
            }
        }

        public static readonly IEnumerable<object[]> filters = new List<object[]>()
        {
            new object[]{ new EmployeeFilter(){ Search = "a" } },
            new object[]{ new EmployeeFilter(){ Page = 1, PageSize = 1, Sorts = new FilterSort[] { new("FirstName", true) } } },
            new object[]{ new EmployeeFilter(){ PageSize = 1, Sorts = new FilterSort[] { new("LastName") } } },
        };

        [Theory]
        [MemberData(nameof(filters))]
        public async Task List_Success(EmployeeFilter filter)
        {
            var domainContext = this.Fixture.ServiceProvider.GetService<DomainContext>();
            var tenant = this.Fixture.AbsoluteTenant;

            List<Employee> employees = new();

            for (int k = 0; k < 10; k++)
            {
                var employee = await GenerateRandomEmployee(domainContext, tenant);
                employees.Add(employee);
            }

            var result = CreateController().List(filter);

            if (filter.PageSize.HasValue)
                Assert.Equal(filter.PageSize, result.Result.Length);
        }
    }
}
