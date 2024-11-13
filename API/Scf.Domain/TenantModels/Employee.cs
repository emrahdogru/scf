using Scf.Models;
using FluentValidation;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System.Globalization;

namespace Scf.Domain.TenantModels
{
    public class Employee : TenantEntity, ISearchableEntity, IPerson
    {
        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string email = string.Empty;
        private User? user = null;
        private Employee? manager = null;
        private EmployeeTitle? title = null;
        private IEnumerable<Group>? groups = null;
        private IEnumerable<ObjectId> groupIds = Array.Empty<ObjectId>();
        private Position? position = null;

        private static readonly CultureInfo cultureEn = System.Globalization.CultureInfo.GetCultureInfo("en-us");

        public Employee(DomainContext context, Tenant tenant) : base(context, tenant)
        {
        }


        [BsonElement]
        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        public string FirstName
        {
            get
            {
                return firstName;
            }
            set
            {
                var parts = (value ?? "").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                //if (this.Language > 0)
                //{
                //    parts = parts.Select(x => x.ToLower(GetCultureInfo()))
                //        .Select(x =>
                //        {
                //            char firstChar = x[0].ToString(GetCultureInfo()).ToUpper(GetCultureInfo())[0];
                //            return $"{firstChar}{x.Substring(1)}";
                //        }).ToArray();
                //}

                firstName = string.Join(" ", parts);
            }
        }

        public string LastName
        {
            get
            {
                return lastName;
            }
            set
            {
                //if (this.Language > 0 && value != null)
                //    value = value.ToUpper(GetCultureInfo());

                lastName = string.Join(" ", (value ?? "").Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        public string Email
        {
            get
            {
                return (email ?? "").Trim().ToLower(cultureEn);
            }
            set
            {
                email = (value ?? "").Trim().ToLower(cultureEn);
            }
        }


        [BsonElement]
        internal ObjectId? UserId { get; private set; }

        /// <summary>
        /// Çalışana bağlı kullanıcı
        /// </summary>
        [BsonIgnore]
        public User? User
        {
            get
            {
                if (!this.UserId.HasValue)
                    return null;

                if (user == null || user.Id != this.UserId)
                {
                    user = Context.Users.FindAsync(this.UserId.Value).Result;
                }

                return user;
            }
            set
            {
                Checkers.CheckTenant(this.Tenant, value);

                this.UserId = value?.Id;
                this.user = null;
            }
        }

        public Languages Language
        {
            get
            {
                return this.User?.Language ?? this.Tenant?.Settings.DefaultLanguage ?? Languages.Turkish;
            }
        }

        /// <summary>
        /// Çalışanın yöneticisinin yöneticisi dönüp yine kendi olur ise kısadevre yapıyor. Bu metod, bu kontrolü sağlıyor.
        /// </summary>
        /// <param name="manager">Yönetici</param>
        /// <returns>Eğer kısadevre var ise true döner.</returns>
        public async Task<bool> IsManagerShortCircuit(Employee? manager)
        {
            if (manager == null || !manager.ManagerId.HasValue)
                return false;

            if (manager.Id == this.Id || manager.ManagerId == this.Id)
                return true;

            // Yöneticinin yöneticisi
            var managerOfManager = await Context.Employees.FindAsync(manager.ManagerId.Value, this.Tenant);

            return await IsManagerShortCircuit(managerOfManager);
        }

        [BsonElement]
        internal ObjectId? ManagerId { get; private set; }

        /// <summary>
        /// Çalışanın yöneticisi
        /// </summary>
        [BsonIgnore]
        public Employee? Manager
        {
            get
            {
                if (ManagerId.HasValue && (manager == null || manager.Id != this.ManagerId))
                {
                    manager = Context.Employees.FindAsync(this.ManagerId.Value, this.Tenant).Result;
                }

                return manager;
            }
            set
            {
                Checkers.CheckTenant(this, value);
                ManagerId = value?.Id;
                manager = null;
            }
        }

        [BsonElement]
        internal ObjectId? TitleId { get; private set; }

        /// <summary>
        /// Çalışanın Unvanı
        /// </summary>
        [BsonIgnore]
        public EmployeeTitle? Title
        {
            get
            {
                if (TitleId.HasValue && (title == null || title.Id != this.TitleId))
                {
                    title = Context.EmployeeTitles.FindAsync(this.TitleId.Value, this.Tenant).Result;
                }

                return title;
            }
            set
            {
                Checkers.CheckTenant(this, value);
                TitleId = value?.Id;
                title = null;
            }
        }

        [BsonElement]
        internal IEnumerable<ObjectId> GroupIds
        {
            get
            {
                return groupIds;
            }
            private set
            {
                groupIds = value;
                groups = null;
            }
        }

        /// <summary>
        /// Çalışanın bağlı bulunduğu departmanlar
        /// </summary>
        [BsonIgnore]
        public IEnumerable<Group> Groups
        {
            get
            {
                groups ??= Context.Groups.FindManyAsync(this.Tenant, this.GroupIds.ToArray()).Result;
                return groups;
            }
            set
            {
                groupIds = value?.Where(x => x.TenantId == this.TenantId).Select(x => x.Id) ?? Array.Empty<ObjectId>();
                groups = null;
            }
        }

        [BsonElement]
        internal ObjectId? PositionId { get; private set; }

        /// <summary>
        /// Pozisyon
        /// </summary>
        [BsonIgnore]
        public Position? Position
        {
            get
            {
                if (!PositionId.HasValue)
                    return null;

                if (position == null || PositionId != position.Id)
                    position = Context.Positions.FindAsync(this.PositionId.Value, this.Tenant).Result;

                return position;
            }
            set
            {
                Checkers.CheckTenant(this, value);
                PositionId = value?.Id;
                position = null;
            }
        }

        [BsonElement]
        public bool IsApproved
        {
            get
            {
                return this.User?.IsApproved == true && this.User?.TenantIds.Contains(this.TenantId) == true;
            }
        }

        /// <summary>
        /// Sicil numarası
        /// </summary>
        public string? ExternalId { get; set; }

        [BsonElement]
        protected IEnumerable<string> Keywords
        {
            get
            {
                return KeywordGenerator.GenerateKeywords(this.FullName, this.Email, this.ExternalId);
            }
        }

        IEnumerable<string> ISearchableEntity.Keywords { get { return Keywords; } }


        /// <summary>
        /// Astları
        /// </summary>
        /// <returns></returns>
        public IQueryable<Employee> GetSubordinates()
        {
            return Context.Employees.GetAll(this.Tenant).Where(x => x.ManagerId == this.Id);
        }

        public class Validator : AbstractValidator<Employee>
        {
            public Validator(DomainContext domainContext)
            {
                var l = domainContext.LanguageService;

                RuleFor(x => x.FirstName).NotEmpty().WithMessage(l.Get(x => x.FieldRequired, new { field = l.Get(x => x.FirstName) }));
                RuleFor(x => x.FirstName).MaximumLength(50).WithMessage(l.Get(x => x.FieldMaxLength, new { field = l.Get(x => x.FirstName) }));

                RuleFor(x => x.LastName).NotEmpty().WithMessage(l.Get(x => x.FieldRequired, new { field = l.Get(x => x.LastName) }));
                RuleFor(x => x.LastName).MaximumLength(50).WithMessage(l.Get(x => x.FieldMaxLength, new { field = l.Get(x => x.LastName) }));

                RuleFor(x => x.Email).NotEmpty().WithMessage(l.Get(x => x.FieldRequired, new { field = l.Get(x => x.Email) }));
                RuleFor(x => x.Email).EmailAddress().WithMessage(l.Get(x => x.FieldInvalid, new { field = l.Get(x => x.Email) }));
                RuleFor(x => x.Email).MaximumLength(100).WithMessage(l.Get(x => x.FieldMaxLength, new { field = l.Get(x => x.Email) }));
                RuleFor(x => x.Email).Must((employee, email, ct) => !domainContext.Employees.AsQueryable().Any(x => x.Email == email && x.Id != employee.Id)).WithMessage(l.Get(x => x.EmailAlreadyExist));
                
                RuleFor(x => x.Manager).MustAsync(async (employee, manager, validationContext) => !await employee.IsManagerShortCircuit(manager)).WithMessage(l.Get(x => x.TheManagerCannotBeSubordinateOfTheEmployee));
                RuleFor(x => x.ExternalId).MaximumLength(100).WithMessage(l.Get(x => x.FieldMaxLength, new { field = l.Get(x => x.ExternalId) }));
            }
        }
    }
}
