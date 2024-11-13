using Scf.Domain.TenantModels;
using Scf.Models;
using Scf.Notifications;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Scf.Domain
{
    public class NotificationQueue : Entity
    {
        private IPerson? person = null;

        public NotificationQueue(DomainContext context) : base(context)
        {
        }

        [BsonElement]
        public string Subject { get; internal set; } = null!;
        [BsonElement]
        public string Message { get; internal set; } = null!;
        [BsonElement]
        public NotificationType Type { get; internal set; }
        [BsonElement]
        public Languages Language { get; internal set; }

        [BsonElement]
        internal protected ObjectId? PersonId { get; protected set; }

        [BsonElement]
        internal protected string? PersonType { get; protected set; }

        [BsonIgnore]
        public IPerson? Person
        {
            get
            {
                if (!this.PersonId.HasValue)
                    return null;

                if (person == null || person.Id != this.PersonId)
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    person = this.PersonType switch
                    {
                        nameof(User) => Context.Users.FindAsync(this.PersonId.Value).Result,
                        nameof(Employee) => Context.Employees.FindAnywhereAsync(this.PersonId.Value).Result,
                        _ => throw new InvalidOperationException($"`{this.PersonType}` geçerli bir Person tipi değil."),
                    };
#pragma warning restore CS0618 // Type or member is obsolete
                }

                return person;
            }
            set
            {
                this.person = value;
                this.PersonId = value?.Id;
                this.PersonType = value?.GetType().Name;
            }
        }


        public string[] To { get; set; } = Array.Empty<string>();
    }
}
