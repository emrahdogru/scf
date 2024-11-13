using MongoDB.Bson.Serialization.Attributes;
using Scf.Domain.Interfaces;
using Scf.Domain.SharedModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.TenantModels
{
    public class TaxList : IEnumerable<TaxList.Tax>
    {
        private List<Tax> taxList = new List<Tax>();

        public TaxList(ITaxApplyable entity)
        {
            this.Entity = entity;
        }

        public ITaxApplyable Entity { get; internal set; }

        public double BaseAmount => Entity.BaseAmount;

        public double TotalTaxAmount => this.Sum(x => x.TaxAmount);

        public double TaxInclusiveAmount => this.BaseAmount + this.TotalTaxAmount;

        public IEnumerator<Tax> GetEnumerator()
        {
            return taxList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return taxList.GetEnumerator();
        }

        public class Tax
        {
            private TaxScheme? _taxScheme = null;

            public Tax(TaxScheme taxScheme, TaxList taxlist)
            {
                if (taxScheme == null)
                    throw new ArgumentNullException(nameof(taxScheme));

                if (taxlist == null)
                    throw new ArgumentNullException(nameof(taxlist));

                this.TaxCode = taxScheme.Code;
                _taxScheme = taxScheme;
                this.Taxes = taxlist;
            }

            [BsonIgnore]
            internal TaxList Taxes { get; set; }

            [BsonElement]
            internal string TaxCode { get; private set; }

            [BsonIgnore]
            public TaxScheme TaxScheme
            {
                get
                {
                    if (_taxScheme == null || _taxScheme.Code != this.TaxCode)
                        _taxScheme = TaxScheme.FromCode(TaxCode);
                    return _taxScheme;
                }
                set
                {
                    TaxCode = value.Code;
                    _taxScheme = null;
                }
            }

            /// <summary>
            /// Vergi matrahı
            /// </summary>
            [BsonElement]
            public double BaseAmount => TaxScheme.TaxBaseAmount(Taxes);

            /// <summary>
            /// Vergi oranı
            /// </summary>
            [BsonElement]
            public double Percent { get; set; }

            /// <summary>
            /// Vergi tutarı
            /// </summary>
            [BsonElement]
            public double TaxAmount => BaseAmount / 100 * Percent;

            /// <summary>
            /// Vergi dahil tutar
            /// </summary>
            [BsonElement]
            public double TaxInclusiveAmount => BaseAmount + TaxAmount;
        }
    }
}
