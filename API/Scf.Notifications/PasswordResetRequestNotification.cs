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
    public class PasswordResetRequestNotification : NotificationBase<PasswordResetRequestNotification.DataModel>
    {
        public PasswordResetRequestNotification(AppSettings appSettings, ILanguageService languageService, IPerson person, PasswordResetRequestNotification.DataModel model)
            : base(appSettings, languageService, person, model)
        {
        }

        public override Expression<Func<Lang, L>> Subject => x => x.NotificationPasswordResetRequestSubject;

        public override Expression<Func<Lang, L>> Message => x => x.NotificationPasswordResetRequestMessage;

        public override NotificationType Type => NotificationType.Email;

        public override string? GetUrl()
        {
            return $"{this.GetDomain()}/password/reset/{Model.Id}?key={Model.Key}";
        }

        public class DataModel
        {
            /// <summary>
            /// <c>PasswordResetRequest.Id</c>
            /// </summary>
            public ObjectId Id { get; set; }

            /// <summary>
            /// <c>PasswordResetRequest.GenerateKey()</c>
            /// </summary>
            public string Key { get; set; } = null!;
        }
    }
}
