using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain
{

    public interface IEntity
    {
        ObjectId Id { get; set; }

        IDeleteInfo? Deleted { get; }

        IEntityContext Context { get; set; }

        int Version { get; set; }

        /// <summary>
        /// Kaydı silindi olarak işaretler
        /// </summary>
        /// <param name="user">Kaydı silen kullanıcı</param>
        void Remove(IUser user);
    }
}
