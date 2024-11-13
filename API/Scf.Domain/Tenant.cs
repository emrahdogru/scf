using FluentValidation;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Scf.Domain
{
    public partial class Tenant : Entity, ITenant
    {
        ObjectId[]? _administratorIds = null;
        IEnumerable<User>? _administrators = null;

        public Tenant(DomainContext context) : base(context)
        {
            this.AdministratorIds = Array.Empty<ObjectId>();
        }

        [BsonRequired]
        public string Code { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        [BsonElement]
        internal ObjectId[] AdministratorIds
        {
            get
            {
                _administratorIds ??= Array.Empty<ObjectId>();
                return _administratorIds;
            }
            set
            {
                _administratorIds = value;
                _administrators = null;
            }
        }

        public Setting Settings { get; set; } = new Setting();

        /// <summary>
        /// Hesap yöneticileri. Hesap üzerinde her türlü işlemi yapmaya yetkilidirler.
        /// </summary>
        [BsonIgnore]
        public IEnumerable<User> Administrators
        {
            get
            {
                _administrators ??= Context.Users.FindManyAsync(this.AdministratorIds).Result;
                return _administrators;
            }
            set
            {
                _administratorIds = value?.Select(x => x.Id).ToArray();
            }
        }

        public Dictionary<string, LanguageResources.L>? GetLanguageResource()
        {
            return TenantLanguageResource.Get(Context.Database, this.Id)?.Resource;
        }

        Languages ITenant.DefaultLanguage
        {
            get
            {
                return Settings.DefaultLanguage;
            }
            set
            {
                Settings.DefaultLanguage = value;
            }
        }

        public class Validator : AbstractValidator<Tenant>
        {
            public Validator(DomainContext context)
            {
                var l = context.LanguageService;
                RuleFor(x => x.Title)
                    .NotEmpty().WithMessage(l.Get(x => x.FieldRequired, new { field = l.Get(x => x.TenantTitle) }))
                    .MaximumLength(200).WithMessage(l.Get(x => x.FieldInvalid, new { field = l.Get(x => x.TenantTitle) }));

                RuleFor(x => x.Code)
                    .NotEmpty().WithMessage(l.Get(x => x.FieldRequired, new { field = l.Get(x => x.TenantCode) }))
                    .Matches("^[a-z_$][a-z_$0-9]*$").WithMessage(l.Get(x => x.FieldInvalid, new { field = l.Get(x => x.TenantCode) }));

                RuleFor(x => x.Settings.DefaultLanguage)
                    .IsInEnum().WithMessage(l.Get(x => x.FieldInvalid, new { field = l.Get(x => x.Language) }))
                    .Must((tenant, x) => tenant.Settings.AvailableLanguages != null && tenant.Settings.AvailableLanguages.Contains(x)).WithMessage(l.Get(f => f.DefaultLanguageMustBeFromAvailableLanguages));
                
                RuleForEach(x => x.Settings.AvailableLanguages)
                    .IsInEnum().WithMessage(l.Get(x => x.FieldInvalid, new { field = l.Get(x => x.Language) }));
            }
        }
    }
}
