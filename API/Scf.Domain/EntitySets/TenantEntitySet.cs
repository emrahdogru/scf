using Scf.Database;

namespace Scf.Domain.EntitySets
{
    public class TenantEntitySet : EntitySet<Tenant>
    {
        public TenantEntitySet(EntityContext entityContext, string collectionName, int version)
            : base(entityContext, collectionName, version)
        {
        }

        public Tenant? FindByCode(string code)
        {
            code = code?.ToLower(System.Globalization.CultureInfo.GetCultureInfo("en-us")) ?? throw new ArgumentNullException(nameof(code));
            return GetAll(true).FirstOrDefault(x => x.Code == code);
        }
    }
}
