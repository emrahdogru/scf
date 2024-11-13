using Scf.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.Services
{
    public class ItemService : CardService
    {
        public ItemService(DomainContext context, ISessionService sessionService) : base(context, sessionService)
        {
        }

        internal override Expression<Func<DomainContext, IBaseEntitySet>> EntitySet => x => x.Items;
    }
}
