using Scf.Domain;
using Scf.Domain.TenantModels;
using Scf.Models.Summaries;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models.Forms
{
    public class EmployeeForm : BaseTenantEntityForm<Employee>
    {
        public EmployeeForm() { }

        public EmployeeForm(Employee employee)
            :base(employee)
        {
            this.FirstName = employee.FirstName;
            this.LastName = employee.LastName;
            this.Email = employee.Email;
            this.ManagerId = employee.Manager?.Id;
            this.TitleId = employee.Title?.Id;
            this.GroupIds = employee.Groups?.Select(x => x.Id).ToArray();
            this.PositionId = employee.Position?.Id;
            this.User = employee.User == null ? null : new UserProfileSummary(employee.User);
        }

        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public ObjectId? ManagerId { get; set; }
        public ObjectId[]? GroupIds { get; set; }
        public ObjectId? PositionId { get; set; }
        public ObjectId? TitleId { get; set; }

        public UserProfileSummary? User { get; }

        public override async Task Bind(DomainContext context, User user, Employee entity)
        {
            await base.Bind(context, user, entity);
            entity.FirstName = FirstName;
            entity.LastName = LastName;
            entity.Email = Email;
            entity.Groups = this.GroupIds == null ? Array.Empty<Group>() : await context.Groups.FindManyOrThrowAsync(entity.Tenant, this.GroupIds);
            entity.Manager = this.ManagerId.HasValue ? await context.Employees.FindOrThrowAsync(this.ManagerId.Value, entity.Tenant) : null;
            entity.Position = this.PositionId.HasValue ? await context.Positions.FindOrThrowAsync(this.PositionId.Value, entity.Tenant) : null;
            entity.Title = this.TitleId.HasValue ? await context.EmployeeTitles.FindOrThrowAsync(this.TitleId.Value, entity.Tenant) : null;
        }
    }
}
