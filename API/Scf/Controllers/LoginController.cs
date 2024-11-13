using Scf.Domain;
using Scf.Models.Forms;
using Scf.Models.Results;
using Scf.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Scf.Domain.TenantModels;
using FluentValidation;
using System.Linq.Dynamic.Core;
using MongoDB.Driver;
using System.Linq;
using Scf.Models.Summaries;

namespace Scf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        readonly ISessionService sessionService;
        readonly DomainContext domainContext;
        readonly UserService userService;

        public LoginController(DomainContext domainContext, ISessionService sessionService, UserService userService)
        {
            this.sessionService = sessionService;
            this.domainContext = domainContext;
            this.userService = userService;

            //var user = domainContext.Users.FindByEmailAsync("emrahdogru@gmail.com").Result;
            //user.SetPassword("Test-1234");
            //domainContext.Users.SaveAsync(user).Wait();
        }

        [HttpPost]
        public async Task<TokenResult> Post(LoginForm form)
        {
            await new LoginForm.Validator(domainContext).ValidateAndThrowAsync(form);

            var token = await userService.Login(form);

            if(token == null)
                throw new UserException(domainContext.LanguageService.Get(x => x.LoginFailure));

            return new TokenResult(token);
        }

        [HttpGet]
        public TokenResult Get()
        {
            var token = sessionService.Token;
            return new TokenResult(token);
        }
    }
}
