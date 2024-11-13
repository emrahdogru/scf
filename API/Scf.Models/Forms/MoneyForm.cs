using Scf.Domain.SharedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models.Forms
{
    public class MoneyForm
    {
        public MoneyForm(Money money)
        {
            this.Amount = money?.Amount ?? 0;
            this.CurrencyCode = money?.Currency?.Code ?? Scf.Domain.ScfApp.DefaultCurrencyCode;
        }

        public double Amount { get; set; }
        public string CurrencyCode { get; set; }
    }
}
