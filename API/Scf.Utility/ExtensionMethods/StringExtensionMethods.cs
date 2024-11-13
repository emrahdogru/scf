using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace System
{
    public static class StringExtensionMethods
    {
        private static readonly Regex TextTemplateRegEx = new(@"{(?<prop>.+?)}", RegexOptions.Compiled);

        public static string FormatTemplate(this string templateString, object model)
        {
            if (model == null)
            {
                return templateString;
            }

            PropertyInfo[] properties = model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            if (!properties.Any())
            {
                return templateString;
            }

            return TextTemplateRegEx.Replace(
                templateString,
                match =>
                {
                    string matchedString = match.Groups["prop"].Value;
                    var parts = matchedString.Split(':', ',');
                    string propertyName = parts[0];
                    bool hasFormat = parts.Length > 1;

                    PropertyInfo? property = properties.FirstOrDefault(propertyInfo =>
                        propertyInfo.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

                    if (property == null)
                    {
                        return match.Groups[0].Value;
                    }

                    object? value = property.GetValue(model, null);

                    if (value == null)
                    {
                        return string.Empty;
                    }
                    else if (hasFormat)
                    {
                        string format = $"{{{matchedString.Replace(propertyName, "0")}}}";
                        return string.Format(format, value) ?? string.Empty;
                    }
                    else
                    {
                        return value.ToString() ?? string.Empty;
                    }
                });
        }
    }
}
