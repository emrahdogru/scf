using Scf.Domain;
using Scf.Domain.TenantModels;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models.Summaries
{
    public class PositionSummary
    {
        public PositionSummary(Position position)
        {
            this.Id = position.Id;
            this.Name= position.Names;
        }

        public ObjectId Id { get; }
        public MultilanguageText Name { get; }
    }
}
