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
    public class PositionController : EntityWithTenantControllerBase<PositionService, Position, PositionForm, FilterSearch<Position>, PositionSummary>
    {
        public PositionController(DomainContext domainContext, ISessionService sessionService, PositionService entityService)
            : base(domainContext, sessionService, entityService)
        {
        }

        protected override PositionForm CreateForm(Position entity)
        {
            return new PositionForm(entity);
        }

        protected override PositionForm CreateForm()
        {
            return new PositionForm();
        }

        protected override PositionSummary CreateSummary(Position entity)
        {
            return new PositionSummary(entity);
        }
    }
}
