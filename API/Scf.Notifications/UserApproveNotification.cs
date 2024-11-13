using Scf.Domain;
using Scf.Domain.Services;
using Scf.LanguageResources;
using Scf.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Notifications
{
    public class UserApproveNotification : NotificationBase<IUser>
    {
        public UserApproveNotification(AppSettings appSettings, ILanguageService languageService, IPerson person, IUser model) : base(appSettings, languageService, person, model)
        {
        }

        public override NotificationType Type => NotificationType.Email;

        public override Expression<Func<Lang, L>> Subject => x => x.NotificationUserApproveSubject;

        public override Expression<Func<Lang, L>> Message => x => x.NotificationUserApproveMessage;

        public override string? GetUrl()
        {
            return $"{this.GetDomain()}/userApprove/{Model.Id}?key={Model.GenerateValidationKey()}";
        }

    }
}
