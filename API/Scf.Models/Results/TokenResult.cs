using Scf.Domain;
using Scf.Models.Summaries;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models.Results
{
    public class TokenResult
    {
        public TokenResult(Token token) { 
            this.Key = token.Key;
            this.User = new UserProfileSummary(token.User);
            this.Tenants = token.User.GetTenants().ToArray()?.Select(x => new TenantSummary(x));
            this.DefaultTenantId = Tenants?.FirstOrDefault()?.Id;
        }

        public string Key { get; }
        public UserProfileSummary User { get; }
        public IEnumerable<TenantSummary>? Tenants { get; }
        public ObjectId? DefaultTenantId { get; }
    }
}
