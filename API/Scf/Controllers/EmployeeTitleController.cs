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
    public class EmployeeTitleController : EntityWithTenantControllerBase<EmployeeTitleService, EmployeeTitle, EmployeeTitleForm, FilterSearch<EmployeeTitle>, EmployeeTitleSummary>
    {
        public EmployeeTitleController(DomainContext domainContext, ISessionService sessionService, EmployeeTitleService entityService) : base(domainContext, sessionService, entityService)
        {
        }

        protected override EmployeeTitleForm CreateForm(EmployeeTitle entity)
        {
            return new EmployeeTitleForm(entity);
        }

        protected override EmployeeTitleForm CreateForm()
        {
            return new EmployeeTitleForm();
        }

        protected override EmployeeTitleSummary CreateSummary(EmployeeTitle entity)
        {
            return new EmployeeTitleSummary(entity);
        }
    }
}
