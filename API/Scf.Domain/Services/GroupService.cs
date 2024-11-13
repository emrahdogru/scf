using Scf.Database;
using Scf.Domain.TenantModels;
using System.Linq.Expressions;

namespace Scf.Domain.Services
{
    public class GroupService : BaseTenantEntityService<Group, EntitySetWithTenant<Group>>
    {
        public GroupService(DomainContext context, ISessionService sessionService) : base(context, sessionService)
        {
        }

        internal override Expression<Func<DomainContext, IBaseEntitySet>> EntitySet => x => x.Groups;
    }
}
