using Scf.Domain;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models.Summaries
{
    public class TenantSummary
    {
        public TenantSummary(Tenant tenant)
        {
            if (tenant == null)
                throw new ArgumentNullException(nameof(tenant));

            this.Id = tenant.Id;
            this.Code= tenant.Code;
            this.Title = tenant.Title;
        }

        public ObjectId Id { get; }
        public string Code { get; }
        public string Title { get; }
    }
}
