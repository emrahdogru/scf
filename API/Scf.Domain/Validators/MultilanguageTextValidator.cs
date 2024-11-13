using Scf.LanguageResources;
using FluentValidation.Validators;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Domain.Validators
{

    public class MultilanguageTextValidator<T, MultilanguageText> : PropertyValidator<T, MultilanguageText>
    {
        readonly IEnumerable<Languages> availableLanguages;
        readonly bool required = false;
        readonly int? maxLength = null;

        public MultilanguageTextValidator(IEnumerable<Languages> availableLanguages, bool required = false, int? maxLength = null)
        {
            this.availableLanguages = availableLanguages;
            this.required = required;
            this.maxLength = maxLength;
        }

        public override string Name => "MultilanguageTextValidator";

        public override bool IsValid(ValidationContext<T> context, MultilanguageText mlTextObject)
        {
            foreach (var l in availableLanguages)
            {
                string languageCode = LanguageDefinition.Definitions[l].Code;
                var val = typeof(MultilanguageText).GetProperty(languageCode)?.GetValue(mlTextObject) as string;

                if (required && string.IsNullOrWhiteSpace(val))
                    return false;
                else if (maxLength.HasValue && val?.Length > maxLength.Value)
                    return false;
            }

            return true;
        }

        protected override string GetDefaultMessageTemplate(string errorCode) => "'{PropertyName}' invalid.";
    }
}
