using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.Services
{
    public interface IMongoClientService
    {
        IMongoClient Client { get; }
    }
}
