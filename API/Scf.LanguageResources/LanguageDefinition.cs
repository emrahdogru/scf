using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.LanguageResources
{
    public class LanguageDefinition
    {

        private static readonly Dictionary<Languages, LanguageDefinition> languageDefinitions = new() {
            { Languages.Turkish, new LanguageDefinition(Languages.Turkish) },
            { Languages.English, new LanguageDefinition(Languages.English) },
        };

        public static Dictionary<Languages, LanguageDefinition> Definitions { get => languageDefinitions; }

        public static CultureInfo GetCultureInfo(Languages language)
        {
            return language switch
            {
                Languages.Turkish => CultureInfo.GetCultureInfo("tr-TR"),
                Languages.English => CultureInfo.GetCultureInfo("en-us"),
                _ => throw new NotImplementedException($"{language} için CultureInfo tanımlanmadı.")
            };
        }

        internal LanguageDefinition(Languages language)
        {
            this.CultureInfo = GetCultureInfo(language);

            this.Code = CultureInfo.TwoLetterISOLanguageName;
            this.LCID = this.CultureInfo.LCID;
            this.NativeName = CultureInfo.Parent?.NativeName ?? CultureInfo.NativeName;
            this.Name = language.ToString();
        }

        public string Code { get; }
        public string Name { get; }
        public int LCID { get; }
        public string NativeName { get; }

        [JsonIgnore]
        public CultureInfo CultureInfo { get; }

    }
}
