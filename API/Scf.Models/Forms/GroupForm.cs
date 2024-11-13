using Scf.Domain;
using Scf.Domain.TenantModels;
using FluentValidation;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models.Forms
{
    public class GroupForm : BaseTenantEntityForm<Group>
    {
        public GroupForm() : base() { }

        public GroupForm(Group entity) : base(entity)
        {
            this.Name = entity.Names;
            this.ParentId = entity.Parent?.Id;
            this.ManagerId = entity.Manager?.Id;
        }

        [BsonElement("Name_")]
        public MultilanguageText Name { get; set; } = new MultilanguageText();
        public ObjectId? ParentId { get; set; }
        public ObjectId? ManagerId { get; set; }

        public override async Task Bind(DomainContext context, User user, Group entity)
        {
            await base.Bind(context, user, entity);
            entity.Names = this.Name;
            entity.Parent = this.ParentId.HasValue ? await context.Groups.FindOrThrowAsync(this.ParentId.Value, entity.Tenant) : null;
            entity.Manager = this.ManagerId.HasValue ? await context.Employees.FindOrThrowAsync(this.ManagerId.Value, entity.Tenant)  : null;
        }

        public class Validator : AbstractValidator<GroupForm>
        {
            public Validator(DomainContext domainContext)
            {
                var l = domainContext.LanguageService;

                RuleFor(x => x.ParentId).NotEqual(x => x.Id).WithMessage(l.Get(x => x.TheMainGroupCannotBeOneOfTheSubgroups));
            }
        }
    }
}
