using Scf.Domain;
using Scf.Domain.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.Models.Forms
{
    public class ChangePasswordForm : IChangePasswordForm
    {
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;

        public class Validator : AbstractValidator<ChangePasswordForm>
        {
            public Validator(DomainContext context)
            {
                var l = context.LanguageService;
                RuleFor(x => x.OldPassword).NotEmpty().WithMessage(l.Get(x => x.FieldRequired, new { field = l.Get(x => x.Password) }));
                RuleFor(x => x.NewPassword).NotEmpty().WithMessage(l.Get(x => x.FieldRequired, new { field = l.Get(x => x.Password) }));
                RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage(l.Get(x => x.FieldRequired, new { field = l.Get(x => x.Password) }));
                RuleFor(x => x.NewPassword).Equal(x => x.ConfirmPassword).WithMessage(l.Get(x => x.FieldInvalid, new { field = l.Get(x => x.ConfirmPassword) }));
            }
        }
    }
}
