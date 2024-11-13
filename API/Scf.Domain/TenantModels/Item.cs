using MongoDB.Bson.Serialization.Attributes;
using Scf.Domain.SharedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.TenantModels
{
    [BsonKnownTypes(
        typeof(ItemAsStock),
        typeof(ItemAsService)
        )]
    public abstract class Item : BaseCard, ISearchableEntity
    {
        private Unit? _unit = null;

        public Item(DomainContext context, Tenant tenant) : base(context, tenant)
        {
            this.ListPrice = new Money(0, this.Tenant.Settings.DefaultCurrency);
        }

        [BsonElement]
        public string Type
        {
            get
            {
                return this.GetType().Name;
            }
        }

        [BsonElement]
        internal string UnitCode { get; private set; } = ScfApp.DefaultUnitCode;

        [BsonIgnore]
        public Unit Unit
        {
            get
            {
                _unit ??= Unit.FromCode(this.UnitCode) ?? Unit.FromCode(ScfApp.DefaultUnitCode);
                return _unit ?? null!;
            }
            set
            {
                this.UnitCode = value.Code;
                _unit = null;
            }
        }

        public Money ListPrice { get; set; }

        public bool IsActive { get; set; } = true;

        IEnumerable<string> ISearchableEntity.Keywords => KeywordGenerator.GenerateKeywords(this.Code, this.Name);
    }
}
