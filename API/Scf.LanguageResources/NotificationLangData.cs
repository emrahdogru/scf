using Scf.LanguageResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf
{
    public partial class Lang
    {
        /// <summary>
        /// Merhaba <c>{name}</c>
        /// </summary>
        public L Salutation = new(
            tr: "Merhaba {name},",
            en: "Dear {name},"
        );

        /// <summary>
        /// Hesabınızı Onaylayın
        /// </summary>
        public L NotificationUserApproveSubject = new(
            tr: $"{appName} Hesabınızı Onaylayın",
            en: $"Approve Your {appName} Account"
        );

        public L NotificationUserApproveMessage = new(
            tr: $"{appName} kullanıcı kaydınız oluşturuldu. Kullanıcınızı etkinleştirmek için aşağıdaki bağlantıyı takip edin:\r\n\r\n{{url}}",
            en: $"Your {appName} user registration has been created. Follow the link below to activate your user:\r\n\r\n{{url}}"
        );

        /// <summary>
        /// Parola sıfırlama talebiniz
        /// </summary>
        public L NotificationPasswordResetRequestSubject = new(
            tr: $"{appName} Parola Sıfırlama Talebiniz",
            en: $"{appName} Password Reset Request"
        );

        public L NotificationPasswordResetRequestMessage = new(
            tr: @$"Bu e-postayı talebiniz üzerine {appName} tarafından parolanızı sıfırlamanız için gönderildi.
Parolanızı sıfırlamak için lütfen aşağıdaki bağlantıyı takip edin:
<a href=""{{url}}"">{{url}}</a>

Eğer böyle bir talepte bulunmadıysanız lütfen bu iletiyi dikkate almayın ve silin.
",
            en: @$"This e-mail has been sent by {appName} upon your request to reset your password.
To reset your password, please follow the link below:
<a href=""{{url}}"">{{url}}</a>

If you have not made such a request, please ignore this message and delete it."
        );

    }
}
