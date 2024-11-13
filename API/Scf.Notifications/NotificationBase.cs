using Scf.Domain;
using Scf.Domain.Services;
using Scf.LanguageResources;
using Scf.Models;
using FluentValidation.Resources;
using System.Linq.Expressions;

namespace Scf.Notifications
{
    public abstract class NotificationBase<T> : INotification<T> where T: class
    {
        protected ILanguageService languageService;
        private readonly AppSettings appSettings;

        public NotificationBase(AppSettings appSettings, ILanguageService languageService, IPerson person, T model)
        {
            this.languageService = languageService;
            this.appSettings = appSettings;
            Language = person.Language;
            Model = model;
            Person = person;
        }

        protected string GetDomain()
        {
            return appSettings.UIDomain;
        }

        public Languages Language { get; set; } = Languages.Turkish;

        public IPerson Person { get; }

        public T Model { get; }

        public abstract Expression<Func<Lang, L>> Subject { get; }
        public abstract Expression<Func<Lang, L>> Message { get; }

        public virtual string? GetUrl()
        {
            return null;
        }

        public string GenerateSubject()
        {
            return languageService.Get(this.Subject, this.Language, new { url = this.GetUrl(), model = this.Model });
        }

        public string GenerateMessage()
        {
            return languageService.Get(this.Message, this.Language, new { url = this.GetUrl(), model = this.Model });
        }

        public abstract NotificationType Type { get; }

    }
}