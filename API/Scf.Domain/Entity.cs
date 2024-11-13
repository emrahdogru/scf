using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Scf.Domain
{

    public abstract class Entity : IEntity
    {
        private ObjectId _id = ObjectId.Empty;

        public Entity(DomainContext context)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [Newtonsoft.Json.JsonMergeKey]
        [BsonId(IdGenerator = typeof(MongoDB.Bson.Serialization.IdGenerators.ObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public ObjectId Id
        {
            get
            {
                if (_id == ObjectId.Empty)
                    _id = ObjectId.GenerateNewId();
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        [JsonIgnore]
        [BsonElement]
        public DeleteInfo? Deleted { get; private set; }

        IDeleteInfo? IEntity.Deleted { get => this.Deleted; }

        [BsonExtraElements]
        protected BsonDocument? ExtraElements { get; set; }

        [BsonElement]
        [JsonIgnore]
        public DateTime CreateDate { get; internal set; } = DateTime.UtcNow;

        [BsonIgnore]
        internal DomainContext Context { get; private set; }
        IEntityContext IEntity.Context { get => (IEntityContext)this.Context; set => this.Context = (DomainContext)value; }

        [BsonElement]
        protected int Version { get; set; }

        int IEntity.Version { get => Version; set => Version = value; }



        /// <summary>
        /// Kaydı silindi olarak işaretler. Kayıt veritabanında durmaya devam eder ancak özellikle silinmiş kayıtlar talep edilmedikçe dönmez.
        /// </summary>
        /// <param name="user">Silme işlemini yapan kullanıcı</param>
        public virtual void Remove(IUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (this is IValidateDelete validateDeleteObj)
                validateDeleteObj.ValidateDelete();

            this.Deleted = new DeleteInfo()
            {
                Date = DateTime.Now,
                IsDeleted = true,
                UserId = user.Id
            };
        }

        void IEntity.Remove(IUser user)
        {
            this.Remove(user);
        }
    }
}
