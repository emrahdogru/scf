using Scf.Domain;
using Scf.Domain.TenantModels;
using FluentValidation;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models.Forms
{
    public class PositionForm : BaseTenantEntityForm<Position>
    {
        public PositionForm():base() { }

        public PositionForm(Position entity) : base(entity)
        {
            this.Name = entity.Names;
        }

        public MultilanguageText Name { get; set; } = new MultilanguageText();

        public override async Task Bind(DomainContext context, User user, Position entity)
        {
            await base.Bind(context, user, entity);
            entity.Names = this.Name;
        }

        public class Validator : AbstractValidator<PositionForm>, IValidator<PositionForm>
        {
            public Validator(DomainContext context)
            {
                var l = context.LanguageService;
                RuleFor(x => x.Name).NotNull().WithMessage(l.Get(x => x.FieldRequired, new { field = l.Get(x => x.PositionName) }));
            }
        }
    }
}
