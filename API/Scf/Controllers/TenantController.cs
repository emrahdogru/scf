using Scf.Domain.Services;
using Scf.Domain;
using Scf.Models.Summaries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Scf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly DomainContext domainContext;
        private readonly ISessionService sessionService;

        public TenantController(DomainContext domainContext, ISessionService sessionService)
        {
            this.domainContext = domainContext;
            this.sessionService = sessionService;
        }

        [HttpGet("detail/{id}")]
        public async Task<TenantDetailSummary> Detail(ObjectId id)
        {
            sessionService.CheckUser();
            var user = sessionService.User;
            var tenant = await domainContext.Tenants.FindAsync(id);

            if (tenant == null)
                throw new EntityNotFountException(nameof(Tenant), id);

            var l = domainContext.LanguageService;

            if (!user.IsInTenant(tenant))
                throw new UserAuthorizationException(l.Get(x => x.UserNotAuthorizedOnThisTenant));

            return new TenantDetailSummary(tenant, user);
        }
    }
}
