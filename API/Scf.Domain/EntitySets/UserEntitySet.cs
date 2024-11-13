using Scf.Database;
using MongoDB.Driver;

namespace Scf.Domain.EntitySets
{
    public class UserEntitySet : EntitySet<User>
    {
        public UserEntitySet(EntityContext entityContext, string collectionName, int version)
            : base(entityContext, collectionName, version)
        { }

        public async Task<User?> FindByEmailAsync(string email)
        {
            email = email?.ToLower(System.Globalization.CultureInfo.GetCultureInfo("en-us")) ?? throw new ArgumentNullException(nameof(email));

            var user = FindInEntityTable(x => x.Email == email).FirstOrDefault();

            if (user == null)
            {
                var results = await GetMongoCollection().FindAsync(x => x.Email == email);
                user = await results.SingleOrDefaultAsync<User>();

                if (user != null)
                    this.SetToEntityTable(user);
            }

            return user;
        }

        /// <summary>
        /// Tenant'a bağlı kullanıcılar
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IQueryable<User> FindByTenant(ITenant tenant)
        {
            if(tenant == null)
                throw new ArgumentNullException(nameof(tenant));

            return GetAll().Where(x => x.TenantIds.Contains(tenant.Id));
        }

    }
}
