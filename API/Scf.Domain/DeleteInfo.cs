using MongoDB.Bson;

namespace Scf.Domain
{
    public class DeleteInfo : IDeleteInfo
    {
        public bool IsDeleted { get; internal set; } = false;
        public DateTime? Date { get; internal set; }
        public ObjectId UserId { get; internal set; }
    }
}
