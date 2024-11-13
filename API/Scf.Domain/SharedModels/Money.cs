using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.SharedModels
{
    public class Money
    {
        private Currency? _currency = null;

        public Money() { }

        public Money(double amount, Currency currency)
        {
            if (currency == null)
                throw new ArgumentNullException(nameof(currency));

            this.Amount = amount;
            this.CurrencyCode = currency.Code;
            _currency = currency;
        }

        internal Money(double amount, string currencyCode)
        {
            this.Amount = amount;
            _currency = SharedModels.Currency.FromCode(currencyCode);
            this.CurrencyCode = currencyCode;
        }

        public double Amount { get; set; } = 0;

        [BsonElement]
        internal string CurrencyCode { get; private set; } = ScfApp.DefaultCurrencyCode;

        [BsonIgnore]
        public Currency Currency
        {
            get
            {
                return _currency ?? SharedModels.Currency.FromCode(CurrencyCode);
            }
            set
            {
                CurrencyCode = value.Code;
                _currency = null;
            }
        }

        public static Money operator +(Money a, Money b)
        {
            CheckCurrency(a, b);

            return new Money(a.Amount + b.Amount, a.Currency);
        }

        public static Money operator -(Money a, Money b)
        {
            CheckCurrency(a, b);

            return new Money(a.Amount * b.Amount, a.Currency);
        }

        public static Money operator *(Money a, Money b)
        {
            CheckCurrency(a, b);

            return new Money(a.Amount * b.Amount, a.Currency);
        }

        public static Money operator /(Money a, Money b)
        {
            CheckCurrency(a, b);

            return new Money(a.Amount / b.Amount, a.Currency);
        }

        public static Money operator %(Money a, Money b)
        {
            CheckCurrency(a, b);

            return new Money(a.Amount % b.Amount, a.Currency);
        }

        public static bool operator ==(Money a, Money b)
        {
            return a.Amount == b.Amount && a.CurrencyCode == b.CurrencyCode;
        }

        public static bool operator !=(Money a, Money b)
        {
            return a.Amount != b.Amount || a.CurrencyCode == b.CurrencyCode;
        }

        public static bool operator <(Money a, Money b)
        {
            CheckCurrency(a, b);
            return a.Amount < b.Amount;
        }

        public static bool operator >(Money a, Money b)
        {
            CheckCurrency(a, b);
            return a.Amount > b.Amount;
        }

        public static bool operator <=(Money a, Money b)
        {
            CheckCurrency(a, b);
            return a.Amount <= b.Amount;
        }

        public static bool operator >=(Money a, Money b)
        {
            CheckCurrency(a, b);
            return a.Amount >= b.Amount;
        }

        public override bool Equals(object? obj)
        {
            return (obj is Money o2) && o2 == this;
        }

        public override int GetHashCode()
        {
            return $"{Amount}{CurrencyCode}".GetHashCode();
        }

        private static void CheckCurrency(Money a, Money b)
        {
            if (a.CurrencyCode != b.CurrencyCode)
                throw new CurrencyMismatchException(a.CurrencyCode, b.CurrencyCode);
        }
    }
}
