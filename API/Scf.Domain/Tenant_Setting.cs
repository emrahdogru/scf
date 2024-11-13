using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using MongoDB.Bson.Serialization.Attributes;
using Scf.Domain.SharedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain
{
    public partial class Tenant
    {


        public class Setting
        {
            private IEnumerable<Currency>? _availableCurrencies = null;

            /// <summary>
            /// Varsayılan dil
            /// </summary>
            public Languages DefaultLanguage { get; set; } = Languages.Turkish;

            /// <summary>
            /// Bu hesap tarafından kullanılabilecek diller.
            /// </summary>
            public IEnumerable<Languages> AvailableLanguages { get; set; } = new Languages[] { Languages.Turkish, Languages.English };


            [BsonElement]
            internal string DefaultCurrencyCode { get; private set; } = "TRY";

            /// <summary>
            /// Varsayılan para birimi
            /// </summary>
            [BsonIgnore]
            public Currency DefaultCurrency
            {
                get
                {
                    return Currency.FromCode(this.DefaultCurrencyCode) ?? null!;
                }
                set
                {
                    this.DefaultCurrencyCode = value.Code;
                }
            }

            [BsonElement]
            internal IEnumerable<string> AvailableCurrencyCodes { get; private set; } = new string[] { "TRY", "USD", "EUR" };

            /// <summary>
            /// Kullanılan para birimleri
            /// </summary>
            [BsonIgnore]
            public IEnumerable<Currency> AvailableCurrencies
            {
                get
                {
                    if(_availableCurrencies == null)
                        _availableCurrencies = this.AvailableCurrencyCodes?.Select(x => Currency.FromCode(x)) ?? Array.Empty<Currency>();

                    return _availableCurrencies;
                }
                set
                {
                    AvailableCurrencyCodes = value?.Select(x => x.Code) ?? Array.Empty<string>();
                    _availableCurrencies = null;
                }
            }

        }
    }
}
