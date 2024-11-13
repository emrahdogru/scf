using Scf.Database;
using Scf.Domain.TenantModels;
using System.Linq.Expressions;

namespace Scf.Domain.Services
{
    public class EmployeeTitleService : BaseTenantEntityService<EmployeeTitle, EntitySetWithTenant<EmployeeTitle>>
    {
        public EmployeeTitleService(DomainContext context, ISessionService sessionService) : base(context, sessionService)
        {
        }

        internal override Expression<Func<DomainContext, IBaseEntitySet>> EntitySet => x => x.EmployeeTitles;
    }
}
