using Scf.Database;
using Scf.Domain.TenantModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.Services
{
    public abstract class CardService : BaseTenantEntityService<BaseCard, EntitySetWithTenant<BaseCard>>
    {
        protected CardService(DomainContext context, ISessionService sessionService) : base(context, sessionService)
        {
        }
    }
}
