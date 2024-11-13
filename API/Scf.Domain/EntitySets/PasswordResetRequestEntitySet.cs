using Scf.Database;
using MongoDB.Bson;

namespace Scf.Domain.EntitySets
{
    public class PasswordResetRequestEntitySet : EntitySet<PasswordResetRequest>
    {
        public PasswordResetRequestEntitySet(EntityContext entityContext, string collectionName) : base(entityContext, collectionName, 1)
        {
        }

        [Obsolete("Create(user, isNewUser) metodunu kullanın.", true)]
        public new PasswordResetRequest Create()
        {
            throw new NotImplementedException("Create(user, isNewUser) metodunu kullanın");
        }

        public PasswordResetRequest Create(User user, bool isNewUser = false)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var entity = new PasswordResetRequest((DomainContext)this.Context, user, isNewUser)
            {
                Id = ObjectId.GenerateNewId()
            };

            AddToEntityTable(entity);

            return entity;
        }
    }
}
