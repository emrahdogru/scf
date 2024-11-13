using MongoDB.Bson;
using MongoDB.Driver;

namespace Scf.Domain.Services
{
    public interface ITenantEntityService<TEntity> where TEntity : class, IEntity
    {
        Task Save(TEntity entity, IClientSessionHandle? sessionHandle = null);
        TEntity Create(ITenant tenant);
        Task<TEntity?> Find(ObjectId id, ITenant tenant);
        IQueryable<TEntity> GetAll(ITenant tenant);
        IQueryable<TEntity> GetAllWithoutContext(ITenant tenant);

        /// <summary>
        /// Bu Id başka bir kiracıya ait kayıt tarafından kullanılıyor mu?
        /// </summary>
        /// <param name="id">Entity Id</param>
        /// <returns>Bu entity Id değeri, başka kiracıya ait bir entity tarafından kullanılıyor ise true döner</returns>
        bool IsFreeObjectId(ObjectId id);
    }
}