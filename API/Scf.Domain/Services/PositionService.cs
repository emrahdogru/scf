using Scf.Database;
using Scf.Domain.TenantModels;
using System.Linq.Expressions;

namespace Scf.Domain.Services
{
    public class PositionService : BaseTenantEntityService<Position, EntitySetWithTenant<Position>>
    {
        public PositionService(DomainContext context, ISessionService sessionService) : base(context, sessionService)
        {
        }

        internal override Expression<Func<DomainContext, IBaseEntitySet>> EntitySet => x => x.Positions;
    }
}
