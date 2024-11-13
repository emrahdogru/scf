using Scf.Database;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Scf.Domain.Services
{
    public abstract class BaseTenantEntityService<TEntity, TEntitySet> : BaseService<TEntity, TEntitySet>, ITenantEntityService<TEntity>
        where TEntity : class, IEntity
        where TEntitySet : class, IEntitySetWithTenant<TEntity>
    {
        public BaseTenantEntityService(DomainContext context, ISessionService sessionService)
            : base(context, sessionService)
        {
        }

        public virtual TEntity Create(ITenant tenant)
        {
            if (tenant == null)
                throw new ArgumentNullException(nameof(tenant));

            return GetEntitySet().Create(tenant);
        }

        /// <summary>
        /// Verilen Id numarasına ait kaydı bulur ve döner
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<TEntity?> Find(ObjectId id, ITenant tenant)
        {
            if(tenant == null)
                throw new ArgumentNullException(nameof(tenant));

            return await GetEntitySet().FindAsync(id, tenant);
        }

        /// <summary>
        /// Belirtilen kiracıdaki kayıtlar için sorgu döner
        /// </summary>
        /// <param name="tenant">Kiracı</param>
        /// <returns></returns>
        public IQueryable<TEntity> GetAll(ITenant tenant)
        {
            if (tenant == null)
                throw new ArgumentNullException(nameof(tenant));

            return GetEntitySet().GetAll(tenant);
        }

        /// <summary>
        /// Bu metod ile yapılan sorgu sonucu dönen Entity'ler context'e eklenmez
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IQueryable<TEntity> GetAllWithoutContext(ITenant tenant)
        {
            if (tenant == null)
                throw new ArgumentNullException(nameof(tenant));
            return GetEntitySet().GetAllWitoutContext(tenant);
        }


        public bool IsFreeObjectId(ObjectId id)
        {
            return !GetEntitySet().AsQueryable().Any(x => x.Id == id);
        }
    }
}
