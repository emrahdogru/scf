using Scf.Domain;
using Scf.Domain.Services;
using Scf.Models.Forms;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Scf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly DomainContext context;
        private readonly UserService userService;
        private readonly ISessionService sessionService;

        public PasswordController(DomainContext context, UserService userService, ISessionService sessionService)
        {
            this.context = context;
            this.userService = userService;
            this.sessionService = sessionService;
        }

        [HttpPost("change")]
        public async Task Change(ChangePasswordForm form)
        {
            sessionService.CheckUser();
            await new ChangePasswordForm.Validator(context).ValidateAndThrowAsync(form);
            await userService.ChangePassword(sessionService.User, form);
        }

        [HttpPost("resetRequest")]
        public async Task ResetRequest(PasswordResetRequestForm form)
        {
            context.IsAutoTrackChangesEnabled = true;
            var user = await context.Users.FindByEmailAsync(form.Email);

            if (user == null)
                throw new EntityNotFountException("User", ObjectId.Empty);

            await userService.CreatePasswordResetRequest(user);
        }

        private async Task<PasswordResetRequest> GetRequest(ObjectId id, string key)
        {
            var l = context.LanguageService;

            var request = await context.PasswordResetRequests.FindAsync(id);
            if (request == null || request.GenerateKey() != key)
                throw new UserException(l.Get(x => x.FieldInvalid, new { field = l.Get(x => x.ValidationKey) }));

            if (!request.IsValid || request.IsUsed)
                throw new UserException(l.Get(x => x.ThisVerificationLinkNoLongerValid));

            return request;
        }

        [HttpGet("reset/{id}")]
        public async Task ResetGet(ObjectId id, string key)
        {
            await GetRequest(id, key);
        }

        [HttpPost("reset")]
        public async Task ResetPost([FromBody]PasswordResetForm form)
        {
            var validator = new PasswordResetForm.Validator(context);
            await validator.ValidateAndThrowAsync(form);

            var request = await GetRequest(form.RequestId, form.Key);
            await request.SetPassword(form.Password);
        }
    }
}
