using Scf.Domain;
using Scf.Domain.Services;
using Scf.Domain.TenantModels;
using Scf.Models.Filters;
using Scf.Models.Forms;
using Scf.Models.Summaries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Scf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : EntityWithTenantControllerBase<EmployeeService, Employee, EmployeeForm, EmployeeFilter, EmployeeSummary>
    {
        public EmployeeController(DomainContext domainContext, ISessionService sessionService, EmployeeService entityService) : base(domainContext, sessionService, entityService)
        {
        }

        protected override EmployeeForm CreateForm(Employee entity)
        {
            return new EmployeeForm(entity);
        }

        protected override EmployeeForm CreateForm()
        {
            return new EmployeeForm();
        }

        protected override EmployeeSummary CreateSummary(Employee entity)
        {
            return new EmployeeSummary(entity);
        }

        [HttpPost("resaveall")]
        public async Task ResaveAll()
        {
            var tenant = sessionService.Tenant;

            if(tenant == null) {
                return;
            }

            domainContext.ValidateEntitiesBeforeSave = false;
            foreach(var e in entityService.GetAll(tenant))
            {
                await entityService.Save(e);
            }
        }
    }
}
