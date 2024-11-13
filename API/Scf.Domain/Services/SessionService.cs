using Scf.Utility.ExtensionMethods;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;

namespace Scf.Domain.Services
{
    public class SessionService : ISessionService
    {
        readonly ITokenService tokenService;
        readonly IHttpContextService httpContextService;
        readonly DomainContext modelContext;

        Token? token = null;
        Tenant? tenant = null;

        public SessionService(ITokenService tokenService, IHttpContextService httpContextService, DomainContext modelContext)
        {
            this.httpContextService = httpContextService;
            this.tokenService = tokenService;
            this.modelContext = modelContext;
        }

        /// <summary>
        /// Oturuma ait token
        /// </summary>
        public Token Token
        {
            get
            {
                token ??= tokenService.Parse(httpContextService.Token).Result;
                return token;
            }
        }

        /// <summary>
        /// Oturumu açık kullanıcı
        /// </summary>
        public User User { get => Token.User; }

        /// <summary>
        /// İşlem yapılan hesap
        /// </summary>
        public Tenant? Tenant
        {
            get
            {
                if (tenant == null)
                {
                    ObjectId? tenantId = httpContextService.TenantId;
                    if (tenantId.HasValue)
                    {
                        tenant = modelContext.Tenants.FindAsync(tenantId.Value).Result;
                        if (tenant == null)
                            throw new TenantMismatchException(modelContext.LanguageService.Get(x => x.UserNotAuthorizedOnThisTenant));
                    }
                }

                if (tenant != null && this.User != null)
                {
                    if (!this.User.TenantIds.Contains(tenant.Id))
                        throw new TenantMismatchException(modelContext.LanguageService.Get(x => x.UserNotAuthorizedOnThisTenant));
                }

                return tenant;
            }
        }

        public Languages Language
        {
            get
            {
                return User?.Language ?? Tenant?.Settings.DefaultLanguage ?? Languages.Turkish;
            }
        }

        /// <summary>
        /// Mevcut oturumu sonlandırır.
        /// </summary>
        public void Logout()
        {
            if (this.Token != null)
                tokenService.Disable(this.Token);
        }

        public void CheckTenant()
        {
            if (Tenant == null)
                throw new TenantMismatchException(modelContext.LanguageService.Get(x => x.UserNotAuthorizedOnThisTenant));
        }

        public void CheckUser()
        {
            if (User == null)
                throw new UnauthorizedAccessException("User not found");
        }
    }
}
