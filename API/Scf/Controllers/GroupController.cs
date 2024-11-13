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
    public class GroupController : EntityWithTenantControllerBase<GroupService, Group, GroupForm, GroupFilter, GroupSummary>
    {
        public GroupController(DomainContext domainContext, ISessionService sessionService, GroupService entityService) : base(domainContext, sessionService, entityService)
        {
        }

        protected override GroupForm CreateForm(Group entity)
        {
            return new GroupForm(entity);
        }

        protected override GroupForm CreateForm()
        {
            return new GroupForm();
        }

        protected override GroupSummary CreateSummary(Group entity)
        {
            return new GroupSummary(entity);
        }
    }
}
