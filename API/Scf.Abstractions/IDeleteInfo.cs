using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain
{
    public interface IDeleteInfo
    {
        DateTime? Date { get; }
        bool IsDeleted { get; }
        ObjectId UserId { get; }
    }
}
