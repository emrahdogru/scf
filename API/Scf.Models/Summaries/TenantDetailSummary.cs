using Scf.Domain;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models.Summaries
{
    public class TenantDetailSummary
    {
        public TenantDetailSummary(Tenant tenant, User user)
        {
            if (tenant == null)
                throw new ArgumentNullException(nameof(tenant));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            this.Id = tenant.Id;
            this.Code = tenant.Code;
            this.Title = tenant.Title;
            this.AvailableLanguages = tenant.Settings.AvailableLanguages;
            this.DefaultLanguage= tenant.Settings.DefaultLanguage;
        }

        public ObjectId Id { get; }
        public string Code { get; }
        public string Title { get; }

        public IEnumerable<Languages> AvailableLanguages { get; }
        public Languages DefaultLanguage { get; }
    }
}
