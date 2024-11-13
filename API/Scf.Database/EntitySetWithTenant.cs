using Scf.Domain;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Database
{
    public class EntitySetWithTenant<T> : BaseEntitySet<T>, IEntitySetWithTenant<T> where T : class, ITenantEntity
    {
        public EntitySetWithTenant(EntityContext entityContext, string collectionName, int version)
            : base(entityContext, collectionName, version)
        {
        }

        public async Task<T?> FindAsync(ObjectId id, ITenant tenant)
        {
            T? result = default;
            if (!entityTable.TryGetValue(id, out var tableItem))
            {
                var results = await GetMongoCollection().FindAsync(x => x.Id == id && x.TenantId == tenant.Id);
                result = await results.SingleOrDefaultAsync<T>();
                if (result != null)
                    SetToEntityTable(result);
            }
            else
            {
                result = tableItem.Entity;
            }

            if (result != null)
                result.Context = this.Context;

            return result;
        }

        public async Task<T> FindOrThrowAsync(ObjectId id, ITenant tenant)
        {
            return await FindAsync(id, tenant) ?? throw new EntityNotFountException(typeof(T).Name, id);
        }

        [Obsolete("Özel bir sebebi yoksa FindAsync(id tenant) kullan!", false)]
        public async Task<T?> FindAnywhereAsync(ObjectId id)
        {
            T? result = default;
            if (!entityTable.TryGetValue(id, out var tableItem))
            {
                var results = await GetMongoCollection().FindAsync(x => x.Id == id);
                result = await results.SingleOrDefaultAsync<T>();
                if (result != null)
                    SetToEntityTable(result);
            }
            else
            {
                result = tableItem.Entity;
            }

            if (result != null)
                result.Context = this.Context;

            return result;
        }

        public async Task<T[]> FindManyAsync(ITenant tenant, params ObjectId[] ids)
        {
            if (ids == null || ids.Length == 0)
                return Array.Empty<T>();

            List<T> result = new();
            List<ObjectId> nonExistIds = new();

            foreach (var id in ids)
            {
                if (!entityTable.TryGetValue(id, out var tableItem))
                {
                    nonExistIds.Add(id);
                }
                else
                {
                    if (tableItem.Entity != null)
                        result.Add(tableItem.Entity);
                }
            }

            if (nonExistIds.Any())
            {
                var cursor = await GetMongoCollection().FindAsync(x => nonExistIds.Contains(x.Id) && x.TenantId == tenant.Id);
                await cursor.ForEachAsync(x =>
                {
                    x.Context = this.Context;
                    result.Add(x);
                    SetToEntityTable(x);
                });
            }

            return result.ToArray();
        }

        public async Task<T[]> FindManyOrThrowAsync(ITenant tenant, params ObjectId[] ids)
        {
            if (ids == null || ids.Length == 0)
                return Array.Empty<T>();

            var result = await FindManyAsync(tenant, ids);
            var nonexistIds = result.Select(x => x.Id).Where(x => !ids.Contains(x));

            List<EntityNotFountException> exceptions = new();
            foreach(var i in nonexistIds)
            {
                exceptions.Add(new EntityNotFountException(typeof(T).Name, i));
            }

            if (exceptions.Any())
                throw new AggregateException(exceptions);

            return result;
        }

        public T Create(ITenant tenant)
        {
            if (Activator.CreateInstance(typeof(T), Context, tenant) is not T tenantEntity)
                throw new NotImplementedException($"`{typeof(T).FullName}` entity nesnesi oluşturulamadı.");

            tenantEntity.Id = ObjectId.GenerateNewId();

            entityTable[tenantEntity.Id] = new EntityTableItem<T> { 
                Entity = tenantEntity,
                Hash = null
            };

            return tenantEntity;
        }

        public IQueryable<T> GetAll(ITenant tenant, bool includeDeleted = false)
        {
            var result = this.AsQueryable().Where(x => x.TenantId == tenant.Id);

            if (!includeDeleted)
#pragma warning disable CS8602 // Dereference of a possibly null reference. Veritabanı sorgusuna çevirildiği için önemi yok.
                result = result.Where(x => x.Deleted.IsDeleted != true);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            return result;
        }

        public IQueryable<T> GetAllWitoutContext(ITenant tenant, bool includeDeleted = false)
        {
            var result = this.GetMongoCollection().AsQueryable().Where(x => x.TenantId == tenant.Id);

            if (!includeDeleted)
#pragma warning disable CS8602 // Dereference of a possibly null reference. Veritabanı sorgusuna çevirildiği için önemi yok.
                result = result.Where(x => x.Deleted.IsDeleted != true);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            return result;
        }
    }
}
