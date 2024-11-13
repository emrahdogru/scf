namespace Scf.Domain.Services
{
    public interface ISessionService
    {
        Tenant? Tenant { get; }
        Token Token { get; }
        User User { get; }
        Languages Language { get; }
        void Logout();
        void CheckTenant();
        void CheckUser();
    }
}