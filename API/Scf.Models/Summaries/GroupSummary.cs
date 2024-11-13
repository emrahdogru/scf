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
    public class GroupSummary
    {
        public GroupSummary(Group group, bool setManager = true, bool setParent = true) {
            this.Id = group.Id;
            this.Name = group.Names;
            this.Manager = !setManager || group.Manager == null ? null : new EmployeeSummary(group.Manager, false);
            this.Parent = !setParent || group.Parent == null ? null : new GroupSummary(group.Parent, false, false);
        }

        public ObjectId Id { get; }
        public MultilanguageText Name { get; }

        public EmployeeSummary? Manager { get; }
        public GroupSummary? Parent { get; }
    }
}
