using Scf.Domain;
using FluentValidation;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Database
{
    public abstract class BaseEntitySet<T> : IBaseEntitySet<T> where T: class, IEntity
    {
        private readonly string _collectionName;
        private IMongoCollection<T>? _mongoCollection = null;

        private bool isValidatorDefined = false;
        private AbstractValidator<T>? validator = null;

        private static readonly MD5 md5 = MD5.Create();

        /// <summary>
        /// Vertiabanından çekilen nesnelerin tekilliğini sağlamak için bu tabloda tutuyoruz. Aynı Id ile kayıt tekrar
        /// talep edildiğinde öncelikle bu tabloda var ise buradan referansı dönüyoruz, yoksa veritabanından çekerek
        /// bu tabloya ekliyoruz. 
        /// </summary>
        internal Dictionary<ObjectId, EntityTableItem<T>> entityTable = new();


        public BaseEntitySet(EntityContext entityContext, string collectionName, int currentVersion)
        {
            Context = entityContext ?? throw new ArgumentNullException(nameof(entityContext));
            _collectionName = collectionName ?? throw new ArgumentNullException(nameof(collectionName));
            CurrentVersion = currentVersion;
        }

        internal protected EntityContext Context { get; private set; }

        public string CollectionName { get { return _collectionName; } }


        public int CurrentVersion { get; private set; }

        EntityContext IBaseEntitySet.Context => this.Context;


        void IBaseEntitySet<T>.SetToEntityTable(T entity)
        {
            SetToEntityTable(entity);
        }

        bool IBaseEntitySet<T>.IsEntityTableContains(T entity) => IsEntityTableContains(entity);

        protected bool IsEntityTableContains(T entity)
        {
            return entityTable.ContainsKey(entity.Id);
        }

        protected byte[] GetEntityHash(T entity)
        {
            return md5.ComputeHash(entity.ToBson());
        }

        internal protected void SetToEntityTable(T entity)
        {
            byte[]? hash = null;
            if (Context.IsAutoTrackChangesEnabled)
                hash = Utility.Tools.Md5AsByteArray(entity.ToBson());

            entityTable[entity.Id] = new EntityTableItem<T>() { Entity = entity, Hash = hash };

            entity.Context = Context;
        }

        internal protected void AddToEntityTable(T entity)
        {
            entityTable[entity.Id] = new EntityTableItem<T>() { Entity = entity, Hash = null };
            entity.Context = Context;
        }

        internal protected IEnumerable<T> FindInEntityTable(Func<T, bool> predicate)
        {
            if(predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return entityTable.Select(x => x.Value.Entity).Where(predicate);
        }

        IEnumerable<T> IBaseEntitySet<T>.FindInEntityTable(Func<T, bool> predicate) => FindInEntityTable(predicate);

        void IBaseEntitySet<T>.AddToEntityTable(T entity)
        {
            AddToEntityTable(entity);
        }

        internal T? GetFromEntityTable(ObjectId id)
        {
            if (entityTable.TryGetValue(id, out var tableItem))
                return tableItem.Entity;
            return null;
        }

        

        /// <summary>
        /// Nesne veritabanından yüklendikten sonra bir değişikliğe uğradı mı?
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool IsEntityChanged(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entityTable.TryGetValue(entity.Id, out var tableItem))
            {
                if (tableItem.Hash != null)
                {
                    var newhash = GetEntityHash(entity);
                    return !newhash.SequenceEqual(tableItem.Hash);
                }
            }

            return true;
        }

        public IMongoCollection<T> GetMongoCollection()
        {
            _mongoCollection ??= Context.Database.GetCollection<T>(_collectionName);

            return _mongoCollection;
        }

        public IQueryable<T> AsQueryable()
        {
            return new QueryWrapper<T>(GetMongoCollection().AsQueryable(), this);
        }

        public AbstractValidator<T>? Validator
        {
            get
            {
                if (!isValidatorDefined)
                {
                    var validatorType = typeof(T).Assembly.GetType(typeof(T).FullName + "+Validator");

                    if (validatorType != null)
                        validator = Activator.CreateInstance(validatorType, this.Context) as AbstractValidator<T>;

                    isValidatorDefined = true;
                }

                return validator;
            }
        }

        public EntitySetIndex<T>[] Indexes { get; set; } = Array.Empty<EntitySetIndex<T>>();

        /// <summary>
        /// Kayıt veritabanında varsa günceller, yoksa ekler
        /// </summary>
        /// <param name="entity"></param>
        /// <exception cref="Exception"></exception>
        public async Task SaveAsync(T entity, IClientSessionHandle? sessionHandle = null)
        {
            if(entity == null)
                throw new ArgumentNullException(nameof(entity));

            if(entity is IDoBeforeSave doBeforeSave)
                doBeforeSave.DoBeforeSave();

            if (Context.ValidateEntitiesBeforeSave && this.Validator != null)
                await this.Validator.ValidateAndThrowAsync(entity);

            entity.Version = this.CurrentVersion;

            var replaceOptions = new ReplaceOptions() { IsUpsert = true };
            try
            {
                if (sessionHandle != null)
                    await GetMongoCollection().ReplaceOneAsync(sessionHandle, x => x.Id == entity.Id, entity, replaceOptions);
                else
                    await GetMongoCollection().ReplaceOneAsync(x => x.Id == entity.Id, entity, replaceOptions);

                if(Context.IsAutoTrackChangesEnabled)
                    SetToEntityTable(entity);
            }
            catch (MongoWriteException ex)
            {
                
                //A bulk write operation resulted in one or more errors.
                // E11000 duplicate key error index: bastapp.User.$Email dup key: { : "emrahdogru@gmail.com", : ObjectId('57d830d40f42fa2bd8ecad4a') }
                if (ex.WriteError.Code == 11000)
                {
                    throw new UserException("DuplicateKeyError", ex);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Yeni kayıt mı? Veritabanında yok ise yeni kayıttır.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool IsNew(T entity)
        {
            if(entity == null)
                throw new ArgumentNullException(nameof(entity));

            return GetMongoCollection().AsQueryable().Any(x => x.Id == entity.Id);
        }

        /// <summary>
        /// Kaydı silindi olarak işaretler
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="user">Silen kullanıcı</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public virtual async Task Remove(T entity, IUser user)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            entity.Remove(user);
            await SaveAsync(entity);
        }


        /// <summary>
        /// Kaydı veritabanından tamamen siler
        /// </summary>
        /// <param name="entity">Silinecek kayıt.</param>
        /// <exception cref="CanNotDeleteException">Kayıt silinemez ise bu hatayı fırlatır.</exception>
        /// <returns></returns>
        public virtual async Task Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity is IValidateDelete validateDeleteObj)
                validateDeleteObj.ValidateDelete();

            if (this.Context.IsAutoTrackChangesEnabled)
            {
                entityTable[entity.Id] = new EntityTableItem<T>() { Deleted = true, Entity = entity, Hash = null };
            }
            else
            {
                await GetMongoCollection().DeleteOneAsync(x => x.Id == entity.Id);
                entityTable.Remove(entity.Id);
            }
        }

        public async Task SaveChangesAsync(IClientSessionHandle sessionHandle)
        {
            if (!this.Context.IsAutoTrackChangesEnabled)
                throw new ArgumentException("IsAutoTrackEnabled devre dışıyken SaveChanges() kullanılamaz.");

            var deletedEntityIds = this.entityTable.Where(x => x.Value.Deleted).Select(x => x.Key).ToArray();

            if (deletedEntityIds.Any())
            {
                await GetMongoCollection().DeleteManyAsync(sessionHandle, x => deletedEntityIds.Contains(x.Id));

                foreach (var id in deletedEntityIds)
                    entityTable.Remove(id);
            }

            foreach (var i in entityTable)
            {
                if (i.Value.Entity != null && IsEntityChanged(i.Value.Entity))
                {
                    await SaveAsync(i.Value.Entity, sessionHandle);
                }
            }
        }

        public async Task InitializeCollection()
        {
            var indexModels = this.Indexes.Where(x => x != null).Select(x => x.GetIndexModel());

            if(indexModels != null && indexModels.Any())
                await this.GetMongoCollection().Indexes.CreateManyAsync(indexModels);
        }

        public async Task ResaveAllAsync(Action<IEntity?, Exception> onError, bool ignoreVersionCheck = false)
        {
            var data = AsQueryable();

            if (!ignoreVersionCheck)
                data = data.Where(x => x.Version < this.CurrentVersion);

            try
            {
                foreach (var e in data)
                {
                    try
                    {
                        await SaveAsync(e);
                    }
                    catch (Exception ex)
                    {
                        onError(e, ex);
                    }
                }
            }
            catch(Exception ex) {
                var nex = new Exception($"Veri okunamadı: {this.GetType().FullName}", ex);
                onError(null, nex);
            }
        }

    }
}
