using Scf.LanguageResources;
using FluentValidation;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;

namespace Scf.Domain.TenantModels
{
    public class Position : TenantEntity, IValidateDelete, ISearchableEntity
    {
        public Position(DomainContext context, Tenant tenant) : base(context, tenant)
        {
        }

        public MultilanguageText Names { get; set; } = new MultilanguageText();

        [BsonElement]
        protected IEnumerable<string> Keywords
        {
            get
            {
                return KeywordGenerator.GenerateKeywords(this.Names);
            }
        }

        IEnumerable<string> ISearchableEntity.Keywords => this.Keywords;


        void IValidateDelete.ValidateDelete()
        {
            if (Context.Employees.GetAll(this.Tenant).Any(x => x.PositionId == this.Id))
                throw new CannotDeleteException(Context.LanguageService.Get(x => x.ThereAreEmployeesForThisPosition));
        }
    }

    public abstract class Validator : DomainAbstractValidator<Position> 
    {

        public Validator(DomainContext domainContext) : base(domainContext) {

            var l = domainContext.LanguageService;

            RuleFor(x => x.Names)
            .NotNull().WithMessage(l.Get(x => x.FieldRequired, new { field = l.Get(x => x.PositionName) }))
            .MultiLanguageValidate(this.AvailableLanguages, true, 100).WithMessage(l.Get(x => x.FieldInvalid, new { field = l.Get(x => x.PositionName) }));
        }

    }
}
