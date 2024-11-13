using Scf.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Net.Http.Headers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Scf.Domain
{
    public class Token : Entity, IDoBeforeSave
    {

        /// <summary>
        /// Token geçerlilik süresi (Dakika)
        /// </summary>
        public int ValidDuration
        {
            get
            {
                return this.Source switch
                {
                    TokenSource.Web => 50,
                    TokenSource.Mobile => 30 * 24 * 60,// 1 Ay
                    TokenSource.Service => 20,
                    _ => 20,
                };
            }
        }

        internal Token(User user, TokenSource source, TokenKind kind)
            : base(user.Context)
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));
            this.UserId = user.Id;
            this.Source = source;
            this.LastActionDate = DateTime.UtcNow;
            this.Key = Guid.NewGuid().ToString("N", System.Globalization.CultureInfo.InvariantCulture) + Guid.NewGuid().ToString("N", System.Globalization.CultureInfo.InvariantCulture);
            this.Kind = kind;
        }

        private User? _user = null;

        [BsonElement]
        internal ObjectId UserId { get; private set; }

        [BsonIgnore]

        public User User
        {
            get
            {
                if (_user == null || _user.Id != this.UserId)
                    _user = Context.Users.FindAsync(this.UserId).Result;

                if (_user == null)
                    throw new EntityNotFountException(nameof(Context.Users), this.UserId);

                return _user;
            }
        }

        [BsonElement]
        public DateTime LastValidDate
        {
            get
            {
                var result = this.Deleted?.Date ?? this.LastActionDate.AddMinutes(ValidDuration);

                if (Context.AppSettings.TokenAbsoluteTimeout > 0)
                {
                    var tokenAbsoluteTimeout = new TimeSpan(0, Context.AppSettings.TokenAbsoluteTimeout.Value, 0);
                    if (this.Source != TokenSource.Mobile && tokenAbsoluteTimeout.TotalMinutes > 0 && result > this.CreateDate.Add(tokenAbsoluteTimeout))
                        return this.CreateDate.Add(tokenAbsoluteTimeout);
                }
                return result;
            }
        }

        [BsonElement]
        public DateTime LastActionDate { get; private set; }

        public TokenSource Source { get; private set; }

        [BsonElement]
        public string Key { get; private set; }
        public TokenKind Kind { get; private set; }
        [BsonElement]
        public string? RawUrl { get; private set; }
        [BsonElement]
        public string? UrlReferrer { get; private set; }
        [BsonElement]
        public string? UserAgent { get; private set; }
        [BsonElement]
        public string? UserHostAddress { get; private set; }


        public bool IsValid()
        {
            return this.Deleted?.IsDeleted != true && this.LastValidDate > DateTime.UtcNow;
        }

        internal void SetHttpContextInfo(IHttpContextService httpContextService)
        {
            var headers = httpContextService.Headers;
            if (headers != null)
            {
                this.RawUrl = httpContextService.GetEncodedPathAndQuery();

                try
                {
                    this.UrlReferrer = headers[HeaderNames.Referer].ToString();
                }
                catch
                {
                    this.UrlReferrer = "";
                }

                this.UserAgent = headers[HeaderNames.UserAgent].ToString();
                this.UserHostAddress = httpContextService?.RemoteIpAddress?.ToString();
            }
        }

        void IDoBeforeSave.DoBeforeSave()
        {
            this.LastActionDate = DateTime.UtcNow;
        }

        public enum TokenSource
        {
            Web,
            Mobile,
            Service
        }

        public enum TokenKind
        {
            /// <summary>
            /// Parola ile oturum açıldı
            /// </summary>
            Password,


            /// <summary>
            /// 3rd party uygulama üzerinden, SSO şeklinde oturum açıldı
            /// </summary>
            App,

            /// <summary>
            /// Destek için Management portalı üzerinden oturum açıldı.
            /// </summary>
            Maintenance,

            /// <summary>
            /// Harici kimlik sağlayıcı ile oturum açıldı. (Microsoft,Twitter,Facebook vs.)
            /// </summary>
            External
        }
    }
}
