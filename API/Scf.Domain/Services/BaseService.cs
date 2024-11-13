using Scf.Database;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Scf.Domain.Services
{
    public abstract class BaseService<TEntity, TEntitySet>
        where TEntity : class, IEntity
        where TEntitySet : class, IBaseEntitySet<TEntity>
    {
        protected readonly DomainContext context;
        protected readonly ISessionService sessionService;
        private TEntitySet? _entitySet = null;

        public BaseService(DomainContext context, ISessionService sessionService)
        {
            this.context = context;
            this.sessionService = sessionService;
        }

        internal abstract Expression<Func<DomainContext, IBaseEntitySet>> EntitySet { get; }

        internal virtual TEntitySet GetEntitySet()
        {
            _entitySet ??= (TEntitySet)this.EntitySet.Compile()(context);
            return _entitySet;
        }

        /// <summary>
        /// Sadece verilen nesneyi veritabanına kaydeder. Context içindeki diğer nesneler olduğu gibi durur.
        /// </summary>
        /// <param name="entity">Kaydedilecek nesne</param>
        /// <param name="sessionHandle"></param>
        /// <returns></returns>
        public virtual async Task Save(TEntity entity, IClientSessionHandle? sessionHandle = null)
        {
            await GetEntitySet().SaveAsync(entity, sessionHandle);
        }
    }
}
