using Scf.Notifications;

namespace Scf.Domain.Services
{
    public class NotificationService
    {
        readonly DomainContext domainContext;

        public NotificationService(DomainContext domainContext)
        {
            this.domainContext = domainContext;
        }

        /// <summary>
        /// NotificationQueue objesini oluşturur ancak kaydetmez. Transaction içinde SaveChanges() çağırıldığında
        /// diğer tüm nesneler ile birlikte kaydedilir.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="notification"></param>
        /// <returns></returns>
        public NotificationQueue CreateNotification<T>(INotification<T> notification) where T : class
        {
            if(notification == null)
                throw new ArgumentNullException(nameof(notification));

            var queue = domainContext.Notifications.Create();

            queue.Subject = notification.GenerateSubject();
            queue.Message = notification.GenerateMessage();
            queue.Language = notification.Language;

            queue.To = new string[] { notification.Person.Email };
            queue.Person = notification.Person;

            return queue;
        }

        /// <summary>
        /// Notification oluşturur ve Transaction dışında kaydeder.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="notification"></param>
        /// <returns></returns>
        public async Task<NotificationQueue> Send<T>(INotification<T> notification) where T : class
        {
            var q = CreateNotification<T>(notification);
            await domainContext.Notifications.SaveAsync(q);
            return q;
        }
    }
}
