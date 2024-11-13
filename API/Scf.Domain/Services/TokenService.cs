using Microsoft.AspNetCore.Http;
using static Scf.Domain.Token;

namespace Scf.Domain.Services
{
    public class TokenService : ITokenService
    {
        readonly ILanguageService languageService;
        readonly DomainContext modelContext;
        readonly IHttpContextService httpContextService;

        public TokenService(DomainContext modelContext, IHttpContextService httpContextService, ILanguageService languageService)
        {
            this.languageService = languageService;
            this.modelContext = modelContext;
            this.httpContextService = httpContextService;
        }

        public async Task<Token> Parse(string? key)
        {
            if (key == null)
                throw new InvalidTokenException(languageService.Get(x => x.InvalidToken));

            var token = modelContext.Tokens.AsQueryable().FirstOrDefault(x => x.Key == key);

            if (token == null)
                throw new InvalidTokenException(languageService.Get(x => x.InvalidToken));

            if (!token.IsValid())
                throw new InvalidTokenException(languageService.Get(x => x.TokenExpired));

            if (token.Deleted?.IsDeleted == true)
                throw new InvalidTokenException(languageService.Get(x => x.TokenExpired));

            await modelContext.Tokens.SaveAsync(token);

            return token;
        }

        /// <summary>
        /// Kullanıcı için yeni token oluşturur.
        /// </summary>
        /// <param name="user">Kullanıcı</param>
        /// <param name="source">Token talep eden uygulama</param>
        /// <param name="kind">Oturum açma türü</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<Token> Generate(User user, TokenSource source = TokenSource.Web, TokenKind kind = TokenKind.Password)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var context = user.Context;

            var t = new Token(user, source, kind);

            t.SetHttpContextInfo(httpContextService);

            await context.Tokens.SaveAsync(t);

            if (kind != TokenKind.Maintenance)
            {
                // Bu konumdan açılmış geçmiş oturumları sonlandır.
                var oldTokens = context.Tokens.AsQueryable().Where(x => x.UserId == user.Id && x.Source == source && x.Kind != TokenKind.Maintenance && x.Id != t.Id && x.LastValidDate >= DateTime.UtcNow && (x.Deleted == null || !x.Deleted.IsDeleted));
                foreach (var o in oldTokens)
                {
                    o.Remove(user);
                    await context.Tokens.SaveAsync(o);
                }
            }

            return t;
        }

        /// <summary>
        /// Verilen token'ı sonlandırır.
        /// </summary>
        /// <param name="token"></param>
        public async Task Disable(Token token)
        {
            token.Remove(token.User);
            await modelContext.Tokens.SaveAsync(token);
        }
    }
}
