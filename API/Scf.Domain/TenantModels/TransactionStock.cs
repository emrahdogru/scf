using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.TenantModels
{
    public partial class Document
    {
        public class TransactionStock : TenantEntity
        {
            private Item? _item = null;

            public TransactionStock(DomainContext context, Tenant tenant) : base(context, tenant)
            {
            }


            public DateTime Date { get; internal set; }

            public ObjectId DocumentId { get; internal set; }

            internal ObjectId ItemId { get; set; }

            public Item Item
            {
                get
                {
                    if (_item == null || _item.Id != ItemId)
                        _item = Context.Items.FindAsync(this.ItemId, this.Tenant).Result;

                    return _item ?? null!;
                }
                set
                {
                    ItemId = value.Id;
                    _item = null;
                }
            }

            /// <summary>
            /// Miktar
            /// </summary>
            public double Quantity { get; protected internal set; }

            /// <summary>
            /// Birim Fiyat
            /// </summary>
            public double UnitPrice => Amount / Quantity;

            /// <summary>
            /// Tutar
            /// </summary>
            public double Amount { get; protected internal set; }



        }
    }
}
