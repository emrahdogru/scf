using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Scf.Domain.Enums;
using Scf.Domain.Interfaces;
using Scf.Domain.SharedModels;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.TenantModels
{
    public class Invoice : Document
    {
        private AccountTrade? _account = null;
        private IEnumerable<InvoiceRow> _rows = Array.Empty<InvoiceRow>();

        public Invoice(DomainContext context, Tenant tenant) : base(context, tenant)
        {
        }

        [BsonElement]
        internal ObjectId AccountId { get; private set; }

        public AccountTrade Account
        {
            get
            {
                _account ??= Context.Accounts.FindAsync(this.AccountId, this.Tenant).Result as AccountTrade;
                return _account ?? null!;
            }
            set
            {
                AccountId = value.Id;
                _account = null;
            }
        }

        /// <summary>
        /// Vade
        /// </summary>
        public DateTime? Expiry { get; set;}

        public InvoiceType InvoiceType { get; set; }

        /// <summary>
        /// İade mi?
        /// </summary>
        public bool IsReturn { get; set; }

        public IEnumerable<InvoiceRow> Rows
        {
            get
            {
                return _rows;
            }
            set
            {
                _rows = value;
                SetParents(_rows);
            }
        }

        [BsonElement]
        public double SubTotal => this.Rows.Sum(x => x.Amount);
        [BsonElement]
        public double TotalDiscountAmount => this.Rows.Sum(x => x.TotalDiscountAmount);
        [BsonElement]
        public double DiscountedSubTotal => this.Rows.Sum(x => x.DiscountedSubTotal);
        [BsonElement]
        public double TotalTaxAmount => this.Rows.Sum(x => x.TotalTaxAmount);
        [BsonElement]
        public double TaxInclusiveAmount => this.Rows.Sum(x => x.TaxInclusiveAmount);
        [BsonElement]
        public double PayableAmount => this.TaxInclusiveAmount;

        public class InvoiceRow : DocumentRow, IDiscountApplyable, ITaxApplyable
        {
            private Item? _item = null;
            private DiscountList? _discounts = null!;
            private TaxList? _taxes = null;
            private Unit? _unit = null;

            public InvoiceRow(Invoice invoice) : base(invoice)
            {
                this._discounts = new DiscountList(this);
                this.Taxes = new TaxList(this);
                this.UnitCode = ScfApp.DefaultUnitCode;
            }

            [BsonElement]
            internal ObjectId ItemId { get; private set; }

            [BsonIgnore]
            public Item Item
            {
                get
                {
                    _item ??= Context.Items.FindAsync(this.ItemId, this.Tenant).Result;
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
            public double Quantity { get; set; }

            [BsonElement]
            internal string UnitCode { get; private set; }

            [BsonIgnore]
            public Unit Unit
            {
                get
                {
                    _unit ??= Unit.FromCode(this.UnitCode);
                    return _unit ?? null!;
                }
                set
                {
                    UnitCode = value.Code;
                    _unit = null;
                }
            }

            /// <summary>
            /// Birim fiyat
            /// </summary>
            [BsonElement]
            public double UnitPrice
            {
                get
                {
                    return Amount / (Quantity == 0 ? 1 : Quantity);
                }
            }

            /// <summary>
            /// Tutar
            /// </summary>
            public double Amount { get; set; }

            /// <summary>
            /// İndirimler
            /// </summary>
            [BsonElement]
            public DiscountList Discounts
            {
                get
                {
                    if(_discounts == null)
                        _discounts = new DiscountList(this);
                    return _discounts;
                }
                set
                {
                    _discounts = value;
                    _discounts.Entity = this;
                }
            }

            double IDiscountApplyable.BaseAmount => this.Amount;

            /// <summary>
            /// Toplam ıskonto tutarı
            /// </summary>
            [BsonElement]
            public double TotalDiscountAmount => Discounts.TotalDiscountAmount;

            /// <summary>
            /// Iskonto sonrası ara toplam (Vergi matrahı)
            /// </summary>
            [BsonElement]
            public double DiscountedSubTotal => Discounts.DiscountedAmount;

            [BsonElement]
            double ITaxApplyable.BaseAmount => this.DiscountedSubTotal;

            /// <summary>
            /// Vergiler
            /// </summary>
            [BsonElement]
            public TaxList Taxes
            {
                get
                {
                    if(_taxes == null)
                        _taxes = new TaxList(this);
                    return _taxes;
                }
                set
                {
                    _taxes = value;
                    _taxes.Entity = this;
                }
            }

            /// <summary>
            /// Toplam vergi tutarı
            /// </summary>
            [BsonElement]
            public double TotalTaxAmount => Taxes.TotalTaxAmount;

            /// <summary>
            /// Vergiler dahil tutar
            /// </summary>
            [BsonElement]
            public double TaxInclusiveAmount => Taxes.TaxInclusiveAmount;
        }

    }
}
