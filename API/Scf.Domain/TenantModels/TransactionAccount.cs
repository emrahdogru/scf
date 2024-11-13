using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.TenantModels
{
    public partial class Document
    {
        public class TransactionAccount : TenantEntity
        {
            private Account? _account = null;

            public TransactionAccount(DomainContext context, Tenant tenant) : base(context, tenant)
            {

            }

            [BsonElement]
            public DateTime Date { get; internal set; }

            [BsonElement]
            public ObjectId DocumentId { get; internal set; }

            [BsonElement]
            internal ObjectId AccountId { get; private set; }

            [BsonIgnore]
            public Account Account
            {
                get
                {
                    _account ??= Context.Accounts.FindAsync(this.AccountId, this.Tenant).Result;
                    return _account ?? null!;
                }
                set
                {
                    AccountId = value.Id;
                    _account = null;
                }
            }

            /// <summary>
            /// Borç
            /// </summary>
            [BsonElement]
            public double Debt { get; internal set; } = 0;

            /// <summary>
            /// Alacak
            /// </summary>
            [BsonElement]
            public double Credit { get; internal set; } = 0;

            /// <summary>
            /// Vade
            /// </summary>
            [BsonElement]
            public DateTime Expiry { get; internal set; }
        }
    }
}
