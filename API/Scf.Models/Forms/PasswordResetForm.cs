using Scf.Domain;
using FluentValidation;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models.Forms
{
    public class PasswordResetForm
    {
        public ObjectId RequestId { get; set; }
        public string Key { get; set; } = null!;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;

        public class Validator : AbstractValidator<PasswordResetForm>
        {
            public Validator(DomainContext domainContext) {
                var l = domainContext.LanguageService;

                RuleFor(x => x.Key).NotEmpty().WithMessage(l.Get(x => x.FieldInvalid, new { field = l.Get(x => x.ValidationKey) }));
                RuleFor(x => x.Password).NotEmpty().WithMessage(l.Get(x => x.FieldRequired, new { field = l.Get(x => x.Password) }));
                RuleFor(x => x.ConfirmPassword).Matches(x => x.Password).WithMessage(l.Get(x => x.PasswordsDoesntMatch)).When(x => !string.IsNullOrEmpty(x.Password));
            }
        }
    }
}
