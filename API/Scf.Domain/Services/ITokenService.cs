namespace Scf.Domain.Services
{
    public interface ITokenService
    {
        Task<Token> Generate(User user, Token.TokenSource source = Token.TokenSource.Web, Token.TokenKind kind = Token.TokenKind.Password);
        Task<Token> Parse(string? key);
        Task Disable(Token token);
    }
}