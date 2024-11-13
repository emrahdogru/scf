using Scf.LanguageResources;
using MongoDB.Bson;
using System.Dynamic;
using System.Linq.Expressions;

namespace Scf.Domain.Services
{
    public interface ILanguageService
    {
        Languages Language { get; }
        ObjectId? TenantId { get; }

        string Get(Expression<Func<Lang, L>> field, object? formatValues = null);
        string Get(Expression<Func<Lang, L>> field, Languages language, object? formatValues = null);
        string Get(Enum value);
        string Get(Enum value, Languages language);
    }
}