using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Scf.Domain
{
    public class PasswordResetRequest : Entity, IValidatableObject
    {
        private User? _user = null;

        /// <summary>
        /// Parola sıfırlama bağlantısı kaç gün içinde geçersiz olacak.
        /// </summary>
        public const int REQUEST_TIMEOUT_DAYS = 3;

        public PasswordResetRequest(DomainContext domainContext, User user, bool isNewUser) : base(domainContext)
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));
            UserId = user.Id;
            IsNewUserNotification = isNewUser;
            this.Identifier = GenerateIdentifier("");
        }

        [BsonElement]
        internal ObjectId UserId { get; set; }

        public User? User
        {
            get
            {
                if (_user == null || _user.Id != this.UserId)
                    _user = Context.Users.FindAsync(this.UserId).Result;

                return _user;
            }
        }

        [BsonIgnore]
        public string Captcha { get; set; } = null!;

        [BsonIgnore]
        public string Identifier { get; set; } = null!;

        [BsonElement]
        public DateTime? UseDate { get; internal set; }

        public bool IsNewUserNotification { get; set; } = false;

        [BsonElement]
        public bool IsUsed
        {
            get
            {
                return this.UseDate.HasValue;
            }
        }


        /// <summary>
        /// Doğrulama bağlantısı için anahtar oluşturur.
        /// </summary>
        /// <returns></returns>
        public string GenerateKey()
        {
            return Utility.Tools.Sha256($"ï<¼dRqƒ╞█r♪ÄÑ╒│\"ÿL{this.Id}{this.UserId}I╩┼╓─∞║V₧─∞║TcK_╫-╩├b◙¼ñ:Ö█↔6-£Ä«36├ÄA↔636ïAA♣6║A♂");
        }

        public async Task SetPassword(string password)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            if (this.User == null)
                throw new NullReferenceException(nameof(User));

            this.User.SetPassword(password);
            this.UseDate = DateTime.Now;
            await Context.Users.SaveAsync(this.User);
            await Context.PasswordResetRequests.SaveAsync(this);
        }

        public static string GenerateIdentifier(string captchaCode)
        {
            if (captchaCode == null)
                throw new ArgumentNullException(nameof(captchaCode));

            return Utility.Cryptography.EncryptData(captchaCode + "|" + Guid.NewGuid().ToString()) ?? "";
        }

        /// <summary>
        /// Link hala geçerli mi?
        /// </summary>
        [BsonIgnore]
        public bool IsValid
        {
            get { return DateTime.Now <= this.CreateDate.AddDays(REQUEST_TIMEOUT_DAYS); }
        }

        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            var idC = Utility.Cryptography.DecryptData(this.Identifier)?.Split('|').FirstOrDefault();

            if (!this.Captcha.Equals(idC, StringComparison.CurrentCultureIgnoreCase))
            {
                yield return new ValidationResult("Doğrulama kodu geçersiz.", new string[] { nameof(Captcha) });
            }
            else if (this.IsUsed)
            {
                yield return new ValidationResult("Bu parola sıfırlama bağlantısı daha önce kullanıldı. Lütfen yeni bir parola sıfırlama talebi oluşturun.");
            }
            else if (!this.IsValid)
            {
                yield return new ValidationResult("Bu parola sıfırlama bağlantısı zaman aşımına uğradı. Lütfen yeni bir parola sıfırlama talebi oluşturun.");
            }
            else if (this.User == null)
            {
                yield return new ValidationResult("Geçersiz kullanıcı bilgisi.");
            }
            else if (!this.User.IsActive)
            {
                yield return new ValidationResult("Kullanıcı aktif değil.", new string[] { nameof(this.User) });
            }

        }
    }
}
