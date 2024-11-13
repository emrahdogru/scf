using Scf.Domain;
using MongoDB.Bson;
using MongoDB.Driver;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Database
{
    public abstract class EntityContext : IEntityContext
    {
        protected readonly IMongoDatabase _database;
        private IBaseEntitySet[]? _entitySets = null;

        public EntityContext(IMongoDatabase database) {
            _database = database;
        }

        internal protected IMongoDatabase Database { get { return _database; } }

        IMongoDatabase IEntityContext.Database => this.Database;

        /// <summary>
        /// Entitylerdeki değişiklikler otomatik takip edilsin mi?
        /// Toplu kayıt esnasında sadece değişen entityler kaydedilir.
        /// Ek performans yükü getirebilir. Değişiklik takip sürecini
        /// manuel yürütmek için <c>false</c> set edilebilir.
        /// </summary>
        public bool IsAutoTrackChangesEnabled { get; set; } = false;

        /// <summary>
        /// Kaydetmeden önce entity otomatik validate edilsin mi?
        /// Toplu işlemlerde önce tüm kayıtlar için validasyon yapılarak,
        /// hiçbirinde hata yoksa kaydedilebilir. Bu durumda kayıt esnasında
        /// tekardan validasyon yapmamak için <c>false</c> set edilebilir.
        /// </summary>
        public bool ValidateEntitiesBeforeSave { get; set; } = true;

        protected IBaseEntitySet[] EntitySets
        {
            get
            {
                if (_entitySets == null)
                {
                    var type = this.GetType();
                    var _properties = type.GetProperties().Where(x => x.PropertyType.GetInterface(nameof(IBaseEntitySet)) != null).ToArray();
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    _entitySets = _properties?.Select(property => (IBaseEntitySet)property.GetValue(this, null)).ToArray();
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
                }

                return _entitySets ?? Array.Empty<IBaseEntitySet>();
            }
        }

        /// <summary>
        /// Caching aktif olduğunda veritabanından çekilen her entity bir Dictionary üzerinde tutulur
        /// ve sonraki çağrılarında buradan erişilir. Bu şekilde aynı entity'nin bellekte farklı
        /// kopyalarının oluşması da engellenir
        /// </summary>
        public bool EnableCaching { get; set; } = true;

        /// <summary>
        /// Tüm değişiklikleri kaydeder
        /// </summary>
        /// <returns></returns>
        public async Task SaveChanges()
        {
            using var session = await Database.Client.StartSessionAsync();
            session.StartTransaction();

            try
            {
                foreach (var entitySet in EntitySets)
                {
                    await entitySet.SaveChangesAsync(session);
                }

                session.CommitTransaction();
            }
            catch
            {
                session.AbortTransaction();
                throw;
            }
        }

        public async Task InitializeDatabase()
        {
            foreach(var p in this.EntitySets)
            {
                await p.InitializeCollection();
            }
        }

        public async Task ResaveAllAsync(Action<string> message, bool ignoreVersionCheck = false)
        {
            string[] IgnoredEntitySets = new string[] { "Token", "PasswordResetRequest" };

            foreach (var p in this.EntitySets)
            {
                if (IgnoredEntitySets.Contains(p.CollectionName))
                    continue;
                
                var date = DateTime.Now;
                message($"{p.CollectionName} başladı.");
                await p.ResaveAllAsync((e, ex) => {
                    message($"{e?.GetType().Name}/{e?.Id} => ${ex.Message}");
                }, ignoreVersionCheck);
                message($"{p.CollectionName} tamamlandı: {DateTime.Now - date}");
            }
        }

    }
}
