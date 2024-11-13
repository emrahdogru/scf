using Scf.Domain;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models.Forms
{
    public class LoginForm : ILoginForm
    {
        public string Email { get; set; } = "";

        public string Password { get; set; } = "";

        public class Validator : AbstractValidator<LoginForm>
        {
            public Validator(DomainContext context) {
                var l = context.LanguageService;

                RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage(x => l.Get(x => x.FieldInvalid, new { field = l.Get(x => x.Email) }));
                RuleFor(x => x.Password).NotEmpty().WithMessage(x => l.Get(x => x.FieldRequired, new { field = l.Get(x => x.Password) }));
            }
        }
    }
}
