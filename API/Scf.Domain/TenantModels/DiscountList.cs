using Microsoft.VisualBasic;
using Scf.Domain.Enums;
using Scf.Domain.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.TenantModels
{
    public class DiscountList : IEnumerable<DiscountList.Discount>
    {
        private List<DiscountList.Discount> _discounts = new List<Discount>();

        public DiscountList(IDiscountApplyable entity) {
            this.Entity = entity;
        }

        public IDiscountApplyable Entity { get; internal set; }

        /// <summary>
        /// İskonto matrahı
        /// </summary>
        public double BaseAmount => Entity.BaseAmount;

        public double TotalDiscountAmount
        {
            get
            {
                return this.Sum(x => x.DiscountAmount);
            }
        }

        /// <summary>
        /// Iskonto sonrası tutar
        /// </summary>
        public double DiscountedAmount
        {
            get
            {
                return BaseAmount - TotalDiscountAmount;
            }
        }

        public int IndexOf(Discount item)
        {
            return _discounts.IndexOf(item);
        }

        public Discount this[int index]
        {
            get
            {
                return _discounts[index];
            }
            set
            {
                _discounts[index] = value;
            }
        }

        public IEnumerator<Discount> GetEnumerator()
        {
            return _discounts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _discounts.GetEnumerator();
        }

        public class Discount
        {
            private double? _discountAmount;

            public Discount(DiscountList discount) {
                this.Discounts = discount;
            }

            internal DiscountList Discounts { get; set; }

            public DiscountType Type { get; set; }

            /// <summary>
            /// Matrah
            /// </summary>
            public double BaseAmount
            {
                get
                {
                    var index = Discounts.IndexOf(this);
                    if(index == 0)
                        return Discounts.BaseAmount;

                    return Type switch
                    {
                        DiscountType.Amount or DiscountType.PercentFromPreviousSubtotal => Discounts[index - 1].DiscountedAmount,
                        DiscountType.PercentFromBase => Discounts.BaseAmount,
                        _ => throw new NotImplementedException($"`{Type}` indirim tipi için matrah hesaplama şekli tanımlı değil."),
                    };
                }
            }

            public double DiscountAmount
            {
                get
                {
                    return Type switch
                    {
                        DiscountType.Amount => _discountAmount.GetValueOrDefault(0),
                        DiscountType.PercentFromBase or DiscountType.PercentFromPreviousSubtotal => BaseAmount / 100 * this.Percent,
                        _ => throw new NotImplementedException($"`{Type}` indirim tipi için matrah hesaplama şekli tanımlı değil.")
                    };
                }
                set
                {
                    if (Type != DiscountType.Amount)
                        throw new InvalidOperationException($"Yalnızca {DiscountType.Amount} türü için tutar belirlenebilir.");

                    _discountAmount = value;
                }
            }

            public double Percent { get; set; }

            public double DiscountedAmount => BaseAmount - DiscountAmount;
        }
    }
}
