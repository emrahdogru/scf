using Scf.Database;
using Scf.Domain.EntitySets;
using Scf.Models.Forms;
using System.Linq.Expressions;

namespace Scf.Domain.Services
{
    public class UserService : BaseEntityService<User, UserEntitySet>
    {
        readonly ITokenService tokenService;
        readonly NotificationService notificationService;

        public UserService(DomainContext modelContext, ISessionService sessionService, ITokenService tokenService, NotificationService notificationService)
            : base(modelContext, sessionService)
        {
            this.tokenService = tokenService;
            this.notificationService = notificationService;
        }

        internal override Expression<Func<DomainContext, IBaseEntitySet>> EntitySet => x => x.Users;

        public async Task<Token> Login(ILoginForm form)
        {
            if(form == null)
                throw new ArgumentNullException(nameof(form));

            var user = await this.FindByEmail(form.Email);
            if (user?.IsValidPassword(form.Password) == true)
            {
                return await tokenService.Generate(user);
            }

            throw new UserException(context.LanguageService.Get(x => x.LoginFailure));
        }

        public async Task<User?> FindByEmail(string email)
        {
            if(email == null)
                throw new ArgumentNullException(nameof(email));

            return await GetEntitySet().FindByEmailAsync(email);
        }

        public async Task ChangePassword(User? user, IChangePasswordForm form)
        {
            if (user == null)
                throw new UserAuthorizationException();

            if (form == null)
                throw new ArgumentNullException(nameof(form));

            if (user.IsValidPassword(form.OldPassword))
            {
                user.SetPassword(form.NewPassword);
                await context.Users.SaveAsync(user);
            }
            else
            {
                var l = context.LanguageService;
                throw new UserException(l.Get(x => x.FieldInvalid, new { field = l.Get(x => x.Password) }));
            }
        }

        public async Task<PasswordResetRequest> CreatePasswordResetRequest(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var request = context.PasswordResetRequests.Create(user);

            var notification = new Notifications.PasswordResetRequestNotification(context.AppSettings, context.LanguageService, user, new Notifications.PasswordResetRequestNotification.DataModel
            {
                Id = request.Id,
                Key = request.GenerateKey()
            });

            notificationService.CreateNotification(notification);

            await context.SaveChanges();

            return request;
        }
    }
}
