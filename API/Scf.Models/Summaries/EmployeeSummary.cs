using Scf.Domain.TenantModels;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models.Summaries
{
    public class EmployeeSummary
    {
        public EmployeeSummary(Employee employee, bool setManager = true, bool setGroups = true, bool setTitle = true, bool setPosition = true)
        {
            this.Id = employee.Id;
            this.FirstName = employee.FirstName;
            this.LastName = employee.LastName;
            this.Groups = setGroups ? employee.Groups.Select(x => new GroupSummary(x, false, false)) : null;
            this.Position = !setPosition || employee.Position == null ? null : new PositionSummary(employee.Position);
            this.Email = employee.Email;
            this.ExternalId = employee.ExternalId;
            this.Title = setTitle && employee.Title != null ? new EmployeeTitleSummary(employee.Title) : null;
            if(setManager)
                this.Manager = employee.Manager == null ? null : new EmployeeSummary(employee.Manager, false, false, false, false);

            this.ProfilePictureUrl = $"/img/pp/{this.Id.CreationTime.Millisecond % 10}.jpg" ;
            this.IsApproved = employee.IsApproved;
        }

        public ObjectId Id { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public IEnumerable<GroupSummary>? Groups { get; }
        public PositionSummary? Position { get; }
        public EmployeeTitleSummary? Title { get; }
        public string Email { get; }
        public string? ExternalId { get; }
        public bool IsApproved { get; }


        public EmployeeSummary? Manager { get; }

        public string ProfilePictureUrl { get; }
    }
}
