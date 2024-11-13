using Scf.Database;
using MongoDB.Bson;

namespace Scf.Domain.Services
{
    public abstract class BaseEntityService<TEntity, TEntitySet> : BaseService<TEntity, TEntitySet>
        where TEntity : class, IEntity
        where TEntitySet : class, IEntitySet<TEntity>
    {
        public BaseEntityService(DomainContext context, ISessionService sessionService)
            : base(context, sessionService)
        {
        }

        public virtual TEntity Create()
        {
            return GetEntitySet().Create();
        }

        /// <summary>
        /// Verilen Id numarasına ait kaydı bulur ve döner
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<TEntity?> Find(ObjectId id)
        {
            return await GetEntitySet().FindAsync(id);
        }
    }
}
