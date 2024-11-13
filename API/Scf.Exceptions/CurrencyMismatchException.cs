using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf
{
    public class CurrencyMismatchException : Exception
    {
        private string _message = "Döviz tipi uyuşmazlığı";

        public CurrencyMismatchException() { }

        public CurrencyMismatchException(string currencyA, string currencyB) {

            _message = $"{_message}: {currencyA} / {currencyB}";
        }
    }
}
