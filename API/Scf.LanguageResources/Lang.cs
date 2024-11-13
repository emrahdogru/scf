using Scf.LanguageResources;
using System.Linq.Expressions;

namespace Scf
{
    public partial class Lang
    {
        private static readonly Lang _instance = new();
        const string appName = "SCF";



        internal Lang() { }


        public static Lang LanguageInstance => _instance;

        public static string Get(Expression<Func<Lang, L>> field, Languages language, object? formatValues = null)
        {
            string languageCode =  LanguageDefinition.Definitions[language].Code;

            var item = field.Compile().Invoke(LanguageInstance) as L;
            string value = (string?)typeof(L).GetProperty(languageCode)?.GetValue(item) ?? "";

            if (formatValues != null)
                value = value.FormatTemplate(formatValues);

            return value ?? $"[{field.Name}:{languageCode}]";
        }


        public static string GetLanguageCode(Languages language)
        {
            return LanguageDefinition.Definitions[language].Code;
        }

    }
}