using Scf.Models;
using FluentValidation;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace Scf.Domain
{
    public class User : Entity, IUser, IPerson
    {
        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string email = string.Empty;


        const string passwordSalt = "6↔36Ì6-}66¦Ş×(×§}³Í£║T♫æ£ìm ´~╚Ôsdf55636";
        private IEnumerable<PasswordHistoryItem>? passwordHistory = null;
        private static readonly CultureInfo cultureEn = System.Globalization.CultureInfo.GetCultureInfo("en-us");

        public User(DomainContext context)
            : base(context)
        { }

        public User(DomainContext context, Languages language = Languages.Turkish)
            : base(context)
        {
            this.Language = language;
        }

        public override string ToString()
        {
            return this.FullName;
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

                if (this.Language > 0)
                {
                    parts = parts.Select(x => x.ToLower(GetCultureInfo()))
                        .Select(x =>
                        {
                            char firstChar = x[0].ToString(GetCultureInfo()).ToUpper(GetCultureInfo())[0];
                            return $"{firstChar}{x[1..]}";
                        }).ToArray();
                }

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
                if (this.Language > 0 && value != null)
                    value = value.ToUpper(GetCultureInfo());

                lastName = string.Join(" ", (value ?? "").Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        public string Email
        {
            get
            {
                return (email ?? "").ToLower(cultureEn);
            }
            set
            {
                email = (value ?? "").ToLower(cultureEn);
            }
        }

        [BsonElement]
        internal IEnumerable<ObjectId> TenantIds { get; private set; } = Array.Empty<ObjectId>();

        /// <summary>
        /// Kullanıcı e-posta adresi doğrulanmış mı?
        /// </summary>
        [BsonElement]
        public bool IsApproved
        {
            get
            {
                // Eğer kullanıcı gelen maildeki linke tıklayarak parolasını belirledi ise e-posta adresi onaylanmıştır.
                return !string.IsNullOrWhiteSpace(Password);
            }
        }

        /// <summary>
        /// Kullanıcı aktif mi?
        /// </summary>
        public bool IsActive { get; set; } = true;

        public Languages Language { get; set; } = Languages.Turkish;

        [BsonElement]
        private Dictionary<ObjectId, IEnumerable<Authority>> TenantAuthorizations { get; set; } = new Dictionary<ObjectId, IEnumerable<Authority>>();

        public System.Globalization.CultureInfo GetCultureInfo()
        {
            return System.Globalization.CultureInfo.GetCultureInfo(Convert.ToInt32(this.Language));
        }

        [DataType(DataType.Password)]
        [JsonIgnore]
        [BsonElement]
        private string? Password { get; set; }

        private string GetHashedPassword(string password)
        {
            return Utility.Tools.Sha256($"{password}{passwordSalt}{Id}");
        }


        public void SetPassword(string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentOutOfRangeException(nameof(newPassword));

            if (newPassword.Length < 8)
                throw new ArgumentOutOfRangeException(nameof(newPassword), "Parola en az 8 karakter olmalıdır.");

            if (!newPassword.Any(c => Char.IsDigit(c)) || !newPassword.Any(c => Char.IsLetter(c)))
                throw new UserException(Context.LanguageService.Get(x => x.YourPasswordMustContainAtLeastOneLetterAndOneNumber));

            DateTime? lastPasswordChangeDate = PasswordHistory.Skip(1).DefaultIfEmpty().Max(x => x?.CreateDate);

            string hashedPassword = GetHashedPassword(newPassword);

            if (this.PasswordHistory.OrderByDescending(x => x.CreateDate).Take(3).Any(x => x.Password == hashedPassword))
                throw new UserException("Yeni parolanız, son üç parolanızdan farklı olmalı.");

            this.Password = hashedPassword;
            this.PasswordHistory = this.PasswordHistory.Append(new PasswordHistoryItem()
            {
                Password = this.Password,
                CreateDate = DateTime.UtcNow
            });
        }

        public bool IsValidPassword(string password)
        {
            return this.Password != null && this.GetHashedPassword(password).Equals(this.Password, StringComparison.Ordinal);
        }

        /// <summary>
        /// Kullanıcıyı hesaba erişim yetkilisi olarak ekler
        /// </summary>
        /// <param name="tenant"></param>
        public void AddToTenant(Tenant tenant)
        {
            if (tenant == null)
                throw new ArgumentNullException(nameof(tenant));

            this.TenantIds = this.TenantIds.Append(tenant.Id).Distinct().ToArray();
        }

        /// <summary>
        /// Kullanıcnın bu hesaba erişimi var mı
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns></returns>
        public bool IsInTenant(ITenant tenant)
        {
            return this.TenantIds?.Contains(tenant.Id) ?? false;
        }

        public bool HasAuthority(ITenant tenant, Authority authority)
        {
            if (tenant == null)
                throw new ArgumentNullException(nameof(tenant));

            if (!this.IsInTenant(tenant))
                throw new TenantMismatchException();

            if(!this.TenantAuthorizations.ContainsKey(tenant.Id))
                return false;

            return this.TenantAuthorizations[tenant.Id].Contains(authority);
        }

        /// <summary>
        /// Kullanıcının erişim yetkisinin olduğu hesaplar
        /// </summary>
        /// <returns></returns>
        public IQueryable<Tenant> GetTenants()
        {
            return Context.Tenants.GetAll().Where(x => this.TenantIds.Contains(x.Id));
        }

        /// <summary>
        /// Kullanıcının ilişkili olduğu kullanıcılar
        /// </summary>
        /// <returns></returns>
        public IQueryable<TenantModels.Employee> GetEmployees()
        {
            return Context.Employees.AsQueryable().Where(x => x.UserId == this.Id);
        }

        /// <summary>
        /// Kullanıcının <paramref name="tenant"/> üzerindeki ilişkili çalışanı
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns></returns>
        public TenantModels.Employee? GetEmployee(ITenant tenant)
        {
            return Context.Employees.GetAll(tenant).FirstOrDefault(x => x.UserId == this.Id);
        }

        public string GenerateValidationKey()
        {
            return Utility.Tools.Sha256($"{this.Id}╙û♥Ωabº∩)◙-L>(╧┤◄┴╕→bè╕↔∩I♫{this.Email}");
        }

        [BsonElement]
        protected IEnumerable<PasswordHistoryItem> PasswordHistory
        {
            get
            {
                if (passwordHistory == null && this.Password != null)
                {
                    passwordHistory = new PasswordHistoryItem[] {
                        new PasswordHistoryItem(){ Password = this.Password, CreateDate = this.CreateDate }
                    };
                }

                return passwordHistory ?? Array.Empty<PasswordHistoryItem>();
            }
            private set
            {
                passwordHistory = value;
            }
        }

        protected sealed class PasswordHistoryItem
        {
            [BsonElement]
            internal string Password { get; set; } = string.Empty;

            [BsonElement]
            internal DateTime CreateDate { get; set; } = DateTime.UtcNow;
        }

        public enum Authority
        {
            Admin
        }

        public class Validator : AbstractValidator<User>
        {
            public Validator(DomainContext context)
            {
                var l = context.LanguageService;

                RuleFor(x => x.FirstName).NotNull().WithMessage(l.Get(x => x.FieldRequired, new { field = l.Get(x => x.FirstName) }));
                RuleFor(x => x.FirstName).MaximumLength(50).WithMessage(l.Get(x => x.FieldMaxLength, new { field = l.Get(x => x.FirstName), maxLength = 50 }));

                RuleFor(x => x.LastName).NotNull().WithMessage(l.Get(x => x.FieldRequired, new { field = l.Get(x => x.LastName) }));
                RuleFor(x => x.LastName).MaximumLength(50).WithMessage(l.Get(x => x.FieldMaxLength, new { field = l.Get(x => x.LastName), maxLength = 50 }));

                RuleFor(x => x.Email).NotNull().WithMessage(l.Get(x => x.FieldRequired, new { field = l.Get(x => x.Email) }));
                RuleFor(x => x.Email).MaximumLength(100).WithMessage(l.Get(x => x.FieldMaxLength, new { field = l.Get(x => x.Email), maxLength = 100 }));
                RuleFor(x => x.Email).Must((user, email, ct) =>
                     !user.Context.Users.AsQueryable().Any(x => x.Email == email && x.Id != user.Id)
                ).WithMessage(l.Get(x => x.EmailAlreadyExist) + ": {PropertyValue}");
            }
        }
    }
}
