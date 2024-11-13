using Scf.Domain;
using Scf.Models.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Scf.Controllers
{
    [Route("test")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly DomainContext domainContext;

        public TestController(DomainContext domainContext)
        {
            this.domainContext = domainContext;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var tenants = domainContext.Tenants.GetAll();
            foreach(var t in tenants)
            {
                t.Settings.AvailableLanguages = new Languages[] { Languages.Turkish, Languages.English };
                t.Settings.DefaultLanguage = Languages.Turkish;
                await domainContext.Tenants.SaveAsync(t);
            }
            
            return "OK";
        }

        [HttpGet("resaveall")]
        public async Task<string> ResaveAll()
        {
            StringBuilder resul = new StringBuilder();
            var startDate = DateTime.Now;

            domainContext.ValidateEntitiesBeforeSave = false;
            domainContext.EnableCaching = false;
            await domainContext.ResaveAllAsync(x =>
            {
                resul.AppendLine(x);
            }, true);

            //await domainContext.Employees.ResaveAllAsync(true);

            resul.AppendLine((DateTime.Now - startDate).ToString());
            return resul.ToString();
        }

        [HttpGet("initialize/{code}")]
        public async Task<string> InitializeTenant(string code)
        {
            domainContext.IsAutoTrackChangesEnabled= true;
            domainContext.EnableCaching = true;
            domainContext.ValidateEntitiesBeforeSave = false;
            var user = await domainContext.Users.FindByEmailAsync("emrahdogru@gmail.com");
            if (user == null)
            {
                user = domainContext.Users.Create();
                user.Email = "emrahdogru@gmail.com";
                user.FirstName = "Emrah";
                user.LastName = "DOĞRU";
                user.IsActive = true;
                user.SetPassword("Test-1234");
                user.Language = Languages.Turkish;
            }

            var g = new RandomContentGenerator(domainContext);
            var tenant = await g.Initialize(code);

            
            user.AddToTenant(tenant);
            await domainContext.SaveChanges();

             
            return "OK";
        }
    }
}
