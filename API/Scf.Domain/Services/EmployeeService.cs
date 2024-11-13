using Scf.Database;
using Scf.Domain.TenantModels;
using System.Linq.Expressions;

namespace Scf.Domain.Services
{
    public class EmployeeService : BaseTenantEntityService<Employee, EntitySetWithTenant<Employee>>
    {
        public EmployeeService(DomainContext context, ISessionService sessionService) : base(context, sessionService)
        {
        }

        internal override Expression<Func<DomainContext, IBaseEntitySet>> EntitySet => x => x.Employees;
    }
}
