using MongoDB.Bson;
using Scf.Domain.SharedModels;
using Scf.Domain.TenantModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models.Summaries
{
    public class ItemSummary
    {
        public ItemSummary(Item item)
        { 
            this.Id = item.Id;
            this.Type = item.Type;
            this.Name = item.Name;
            this.ListPrice = item.ListPrice;
            this.Unit = item.Unit == null ? null : new UnitSummary(item.Unit);
            this.IsActive = item.IsActive;
        
        }

        public ObjectId Id { get; }
        public string Type { get; }
        public string Name { get; }
        public Money ListPrice { get; }
        public UnitSummary? Unit { get; }
        public bool IsActive { get; }
    }
}
