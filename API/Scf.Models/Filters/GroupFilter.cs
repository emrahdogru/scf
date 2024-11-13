using Scf.Domain;
using Scf.Domain.TenantModels;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models.Filters
{
    public class GroupFilter : FilterSearch<Group>
    {
        public ObjectId[]? ManagerIds { get; set; }
        public ObjectId[]? ParentIds { get; set; }

        public override IQueryable<Group> Apply(IQueryable<Group> query, out int totalRecordCount)
        {
            if (ManagerIds != null && ManagerIds.Any())
                query = query.FilterByManagers(ManagerIds);

            if(ParentIds != null && ParentIds.Any())
                query = query.FilterByParents(ParentIds);

            return base.Apply(query, out totalRecordCount);
        }
    }
}
