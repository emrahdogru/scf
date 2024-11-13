using Scf.LanguageResources;
using Scf.Utility.ExtensionMethods;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Scf.Domain.Services
{
    public class LanguageService : ILanguageService
    {
        Dictionary<string, L>? tenantLanguageResources = null;
        readonly IMongoDatabase mongoDb;

        public LanguageService(IHttpContextService httpContextService, IMongoDbService mongoDbService)
        {
            this.mongoDb = mongoDbService.Database;

            string? cultureName = httpContextService?.Headers?[HeaderNames.ContentLanguage].ToString();
            if (string.IsNullOrWhiteSpace(cultureName))
                cultureName = "tr-TR";

            this.Language = (Languages)System.Globalization.CultureInfo.GetCultureInfo(cultureName).LCID;

            this.TenantId = httpContextService?.TenantId;
        }

        public LanguageService(Languages language, ObjectId? tenantId, IMongoDatabase mongoDatabase)
        {
            this.Language = language;
            this.TenantId = tenantId;
            this.mongoDb = mongoDatabase;
        }

        public Languages Language { get; private set; }

        public ObjectId? TenantId { get; private set; }

        protected Dictionary<string, L> TenantLanguageResources
        {
            get
            {
                if (tenantLanguageResources == null)
                {
                    if (!TenantId.HasValue)
                    {
                        tenantLanguageResources = new Dictionary<string, L>();
                    }
                    else
                    {
                        tenantLanguageResources = TenantLanguageResource.Get(mongoDb, this.TenantId.Value)?.Resource ?? new Dictionary<string, L>();
                    }
                }

                return tenantLanguageResources;
            }
        }

        public string Get(Expression<Func<Lang, L>> field, Languages language, object? formatValues = null)
        {
            if(field == null)
                throw new ArgumentNullException(nameof(field));

            string fieldName = ((System.Linq.Expressions.MemberExpression)field.Body).Member.Name;

            string languageCode = Lang.GetLanguageCode(language);


            if (!TenantLanguageResources.TryGetValue(fieldName, out L? item) || item == null)
            {
                item = field.Compile().Invoke(Lang.LanguageInstance) as L;
            }

            string? value = (string?)typeof(L).GetProperty(languageCode)?.GetValue(item);

            if (value != null && formatValues != null)
                value = value.FormatTemplate(formatValues);

            return value ?? $"[{languageCode}:{fieldName}]";
        }

        public string Get(Expression<Func<Lang, L>> field, object? formatValues = null)
        {
            return Get(field, this.Language, formatValues);
        }

        public string Get(Enum @enum, Languages language)
        {
            string fieldName = $"{@enum.GetType().Name}_{@enum}";
            string languageCode = Lang.GetLanguageCode(language);

            if (!TenantLanguageResources.TryGetValue(fieldName, out L? item) || item == null)
            {
                item = typeof(Lang).GetProperty(fieldName)?.GetValue(Lang.LanguageInstance) as L;
            }

            string? value = (string?)typeof(L).GetProperty(languageCode)?.GetValue(item);
            return value ?? $"[{languageCode}:{fieldName}]";
        }

        public string Get(Enum @enum)
        {
            return Get(@enum, this.Language);
        }
    }
}
