using Scf.Domain.SharedModels;
using Scf.Domain.TenantModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models.Forms
{
    public class ItemForm: BaseCardForm<Item>
    {
        public ItemForm(Item item)
            :base(item)
        {
            this.Type = item.Type;
            this.IsActive = item.IsActive;
            this.ListPrice = new MoneyForm(item.ListPrice);
            this.UnitCode = item.Unit?.Code;
        }

        public string Type { get; set; }
        public bool IsActive { get; set; }
        public MoneyForm ListPrice { get; set; }
        public string? UnitCode { get; set; }
    }

    public class ItemAsStockForm : ItemForm
    {
        public ItemAsStockForm(ItemAsStock item) : base(item)
        {
        }
    }

    public class ItemAsServiceForm : ItemForm
    {
        public ItemAsServiceForm(ItemAsService item) : base(item)
        {
        }
    }
}
