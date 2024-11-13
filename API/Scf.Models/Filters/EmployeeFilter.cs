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
    public class EmployeeFilter : FilterSearch<Employee>
    {
        public IEnumerable<ObjectId>? ManagerIds { get; set; }
        public IEnumerable<ObjectId>? PositionIds { get; set; }
        public IEnumerable<ObjectId>? TitleIds { get; set; }
        public IEnumerable<ObjectId>? GroupIds { get; set; }

        public override IQueryable<Employee> Apply(IQueryable<Employee> query, out int totalRecordCount)
        {
            if (this.ManagerIds != null && this.ManagerIds.Any())
                query = query.FilterByEmployeeManagers(this.ManagerIds.ToArray());

            if(this.PositionIds != null && this.PositionIds.Any())
                query = query.FilterByPositions(this.PositionIds.ToArray());

            if (this.TitleIds != null && this.TitleIds.Any())
                query = query.FilterByTitles(this.TitleIds.ToArray());

            if(this.GroupIds != null && this.GroupIds.Any())
                query = query.FilterByGroups(this.GroupIds.ToArray());



            return base.Apply(query, out totalRecordCount);
        }
    }
}
