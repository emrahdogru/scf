using Scf.Domain;
using FluentValidation;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Database
{
    public class EntitySet<T> : BaseEntitySet<T>, IEntitySet<T> where T : class, IEntity
    {
        public EntitySet(EntityContext entityContext, string collectionName, int version)
            : base(entityContext, collectionName, version)
        {
            
        }

        public virtual T Create()
        {
            if (Activator.CreateInstance(typeof(T), Context) is not T entity)
                throw new NotImplementedException($"`{typeof(T).FullName}` entity nesnesi oluşturulamadı.");

            entity.Id = ObjectId.GenerateNewId();

            AddToEntityTable(entity);

            return entity;
        }

        public virtual void Attach(T entity)
        {
            if (IsEntityTableContains(entity))
                throw new InvalidOperationException("Entity zaten attach edilmiş durumda!");

            AddToEntityTable(entity);
        }

        public async Task<T?> FindAsync(ObjectId id)
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

        public async Task<T> FindOrThrowAsync(ObjectId id)
        {
            return await FindAsync(id) ?? throw new EntityNotFountException(typeof(T).Name, id);
        }

        public async Task<T[]> FindManyAsync(params ObjectId[] ids)
        {
            List<T> result = new();
            List<ObjectId> nonExistIds = new();

            foreach(var id in ids)
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

            if(nonExistIds.Any())
            {
                var cursor = await GetMongoCollection().FindAsync(x => nonExistIds.Contains(x.Id));
                await cursor.ForEachAsync(x =>
                {
                    x.Context = this.Context;
                    result.Add(x);
                    SetToEntityTable(x);
                });
            }

            return result.ToArray();
        }

        public async Task<T[]> FindManyOrThrowAsync(params ObjectId[] ids)
        {
            var result = await FindManyAsync(ids);
            var nonexistIds = result.Select(x => x.Id).Where(x => !ids.Contains(x));

            List<EntityNotFountException> exceptions = new();
            foreach (var i in nonexistIds)
            {
                exceptions.Add(new EntityNotFountException(typeof(T).Name, i));
            }

            if (exceptions.Any())
                throw new AggregateException(exceptions);

            return result;
        }

        public IQueryable<T> GetAll(bool includeDeleted = false)
        {
            var result = this.AsQueryable();

            if (!includeDeleted)
#pragma warning disable CS8602 // Dereference of a possibly null reference. Veritabanı sorgusuna çevirildiği için önemi yok
                result = result.Where(x => x.Deleted.IsDeleted != true);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            return result;
        }
    }
}
