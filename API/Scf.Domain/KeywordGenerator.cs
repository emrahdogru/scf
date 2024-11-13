using Scf.LanguageResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain
{
    internal class KeywordGenerator
    {
        public static string[] GenerateKeywords(params string?[] words)
        {
            List<string> keywords = new List<string>();

            foreach (var l in LanguageDefinition.Definitions)
            {
                foreach (string word in words.Where(x => !string.IsNullOrWhiteSpace(x)).Cast<string>())
                {
                    var w = word.ToLower(l.Value.CultureInfo);
                    keywords.Add(w);
                    keywords.Add(w
                        .Replace('ç', 'c')
                        .Replace('ğ', 'g')
                        .Replace('ı', 'i')
                        .Replace('ö', 'o')
                        .Replace('ş', 's')
                        .Replace('ü', 'u')
                        );
                }
            }

            return keywords.Distinct().ToArray();
        }

        public static string[] GenerateKeywords(MultilanguageText text, string?[] words)
        {
            var tw = GenerateKeywords(text);
            return GenerateKeywords(words.Concat(tw).ToArray());
        }

        public static string[] GenerateKeywords(params MultilanguageText[] texts)
        {
            var words = texts.Where(x => x != null).Select(x => new string[] { x.tr ?? "", x.en ?? "" }).SelectMany(x => x).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            return GenerateKeywords(words);
        }
    }
}
