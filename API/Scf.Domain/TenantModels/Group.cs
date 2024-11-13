using FluentValidation;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;

namespace Scf.Domain.TenantModels
{
    public class Group : TenantEntity, ISearchableEntity
    {
        private Employee? manager = null;
        private Group? parent = null;

        public Group(DomainContext context, Tenant tenant) : base(context, tenant)
        {
        }

        public MultilanguageText Names { get; set; } = new MultilanguageText();

        [BsonElement]
        internal ObjectId? ManagerId { get; set; }

        /// <summary>
        /// Grubun yöneticisi
        /// </summary>
        [BsonIgnore]
        public Employee? Manager
        {
            get
            {
                if (!ManagerId.HasValue)
                    return null;

                if (manager == null || ManagerId != manager.Id)
                    manager = Context.Employees.FindAsync(this.ManagerId.Value, this.Tenant).Result;

                return manager;
            }
            set
            {
                manager = value;
                ManagerId = value?.Id;
            }
        }

        [BsonElement]
        internal ObjectId? ParentId { get; set; }

        /// <summary>
        /// Grubun bağlı olduğu ana grup
        /// </summary>
        [BsonIgnore]
        public Group? Parent
        {
            get
            {
                if (!ParentId.HasValue)
                    return null;

                if (parent == null || parent.Id != this.ParentId)
                    parent = Context.Groups.FindAsync(this.ParentId.Value, this.Tenant).Result;

                return parent;
            }
            set
            {
                parent = value;
                ParentId = value?.Id;
            }
        }

        [BsonElement]
        protected IEnumerable<string> Keywords
        {
            get
            {
                return KeywordGenerator.GenerateKeywords(this.Names);
            }
        }

        IEnumerable<string> ISearchableEntity.Keywords { get { return Keywords; } }

        /// <summary>
        /// Grubun ana grubu dönüp yine kendi olur ise kısadevre yapıyor. Bu metod, bu kontrolü sağlıyor.
        /// </summary>
        /// <param name="parent">Yönetici</param>
        /// <returns>Eğer kısadevre var ise true döner.</returns>
        public async Task<bool> IsParentShortCircuit(Group? parent)
        {
            if (parent == null || !parent.ParentId.HasValue)
                return false;

            if (parent.Id == this.Id || parent.ParentId == this.Id)
                return true;

            // Ana grubun ana grubu
            var parentOfParent = await Context.Groups.FindAsync(parent.ParentId.Value, this.Tenant);

            return await IsParentShortCircuit(parentOfParent);
        }

        /// <summary>
        /// Bu gruba bağlı çalışanlar
        /// </summary>
        /// <returns></returns>
        public IQueryable<Employee> GetEmployees()
        {
            return Context.Employees.GetAll(this.Tenant).Where(x => x.GroupIds.Contains(this.Id));
        }

        public class Validator : DomainAbstractValidator<Group>
        {
            public Validator(DomainContext context)
                :base(context)
            {
                var l = context.LanguageService;

                RuleFor(x => x.Names)
                    .NotNull().WithMessage(l.Get(x => x.FieldRequired, new { field = l.Get(x => x.GroupName) }))
                    .MultiLanguageValidate(this.AvailableLanguages, true, 100).WithMessage(l.Get(x => x.FieldInvalid, new { field = l.Get(x => x.GroupName) }));

                RuleFor(x => x.Parent)
                    .MustAsync(async (group, parent, validationContext) => !await group.IsParentShortCircuit(parent)).WithMessage(l.Get(x => x.TheMainGroupCannotBeOneOfTheSubgroups));
                
                
            }
        }
    }
}
