using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models
{
    public interface IPerson
    {
        public ObjectId Id { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public Languages Language { get; }
    }
}
