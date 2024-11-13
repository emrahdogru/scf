using Scf;
using Scf.Domain;
using Scf.Domain.Validators;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentValidation
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilderOptions<T, TElement> MultiLanguageValidate<T, TElement>(this IRuleBuilder<T, TElement> ruleBuilder, IEnumerable<Languages> availableLanguages, bool required, int? maxLength = null) where TElement : MultilanguageText
        {
            return ruleBuilder.SetValidator(new MultilanguageTextValidator<T, TElement>(availableLanguages, required, maxLength));
        }
    }
}
