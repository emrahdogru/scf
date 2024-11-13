using FluentValidation;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;

namespace Scf.Domain.TenantModels
{
    public class EmployeeTitle : TenantEntity, ISearchableEntity
    {
        public EmployeeTitle(DomainContext context, Tenant tenant) : base(context, tenant)
        {
        }

        public MultilanguageText Names { get; set; } = new MultilanguageText();

        [BsonElement]
        IEnumerable<string> Keywords
        {
            get
            {
                return new string[] { this.Names?.tr?.ToLower() ?? "" };
            }
        }

        IEnumerable<string> ISearchableEntity.Keywords => this.Keywords;

        public class Validator : DomainAbstractValidator<EmployeeTitle>
        {
            public Validator(DomainContext domainContext)
                :base(domainContext)
            {
                var l = domainContext.LanguageService;

                RuleFor(x => x.Names)
                    //.NotNull().WithMessage(l.Get(x => x.FieldRequired, new { field = l.Get(x => x.EmployeeTitle) }))
                    .MultiLanguageValidate(this.AvailableLanguages, true, 100).WithMessage(l.Get(x => x.FieldInvalid, new { field = l.Get(x => x.EmployeeTitle) }));
            }
        }
    }
}
