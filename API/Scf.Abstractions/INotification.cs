using Scf.Domain;
using Scf.LanguageResources;
using Scf.Models;
using System.Linq.Expressions;

namespace Scf.Notifications
{
    public interface INotification<T> where T: class
    {
        /// <summary>
        /// Bildirimin gönderileceği kişi. <c>User</c> ve <c>Employee</c> olabilir.
        /// </summary>
        IPerson Person { get; }

        /// <summary>
        /// Bildirim dili. Varsayılan olarak <para>Person</para> nesnesinden alınır. Sonrasında değiştirilebilir.
        /// </summary>
        Languages Language { get; set; }
        Expression<Func<Lang, L>> Message { get; }
        Expression<Func<Lang, L>> Subject { get; }
        NotificationType Type { get; }

        public string? GetUrl();

        string GenerateSubject();
        string GenerateMessage();
    }

    public enum NotificationType
    {
        Email = 1,
        InApp = 2,
        Firebase = 4
    }
}