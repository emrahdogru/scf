using Scf.Domain.Enums;
using Scf.Domain.SharedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.TenantModels
{
    public class AccountTrade : Account
    {
        public AccountTrade(DomainContext context, Tenant tenant) : base(context, tenant)
        {
        }

        public AccountTradeType Type { get; set; } = AccountTradeType.Customer;

        public string? TaxNumber { get; set; }
        public string? TaxOffice { get; set; }

        public IEnumerable<Address> Addresses { get; set; } = Array.Empty<Address>();

        public UInt16? DefaultInvoiceAddressIndex { get; set; }
        public UInt16? DefaultDispatchAddressIndex { get; set; }
    }
}
