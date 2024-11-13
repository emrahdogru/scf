using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf
{
    public class EntityNotFountException : Exception
    {
        public EntityNotFountException(string entitySetName, string entityId) {
            this.EntitySetName = entitySetName;
            this.EntityId = entityId;
        }

        public EntityNotFountException(string entitySetName, ObjectId entityId)
        {
            this.EntitySetName = entitySetName;
            this.EntityId = entityId.ToString();
        }

        public string EntityId { get; private set; }
        public string EntitySetName { get; private set; }

        public override string Message => $"Kayıt bulunamadı: {EntitySetName}=>{EntityId}";
    }
}
