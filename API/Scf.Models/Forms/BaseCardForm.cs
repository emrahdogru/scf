using Scf.Domain.TenantModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models.Forms
{
    public abstract class BaseCardForm<T>: BaseTenantEntityForm<T> where T : BaseCard
    {
        public BaseCardForm(T card) : base(card)
        {
            this.Code = card.Code;
            this.Name = card.Name;
        }

        public string Code { get; set; }
        public string Name { get; set; }
    }
}
