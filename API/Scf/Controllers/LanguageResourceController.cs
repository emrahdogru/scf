using Scf.Domain.Services;
using Scf.LanguageResources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Scf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguageResourceController : ControllerBase
    {
        private readonly ILanguageService languageService;
        private readonly ISessionService sessionService;

        public LanguageResourceController(ILanguageService languageService, ISessionService sessionService) {
            this.languageService = languageService;
            this.sessionService = sessionService;
        }

        [HttpGet("{languageCode}")]
        public Dictionary<string, string> Get(string languageCode)
        {
            var props = Scf.Lang.LanguageInstance.GetType().GetFields().Where(x => x.FieldType.Name == nameof(Lc)).ToArray();
            var lctype = typeof(Lc);

            Dictionary<string, string> result = new();

            props.AsParallel().ForAll(p =>
            {
                var lc = p.GetValue(Lang.LanguageInstance);

                string? value = lctype.GetProperty(languageCode)?.GetValue(lc)?.ToString();

                if(value != null)
                    result[p.Name] = value;

            });

            return result;
        }

        [HttpGet("languages")]
        public Dictionary<Languages, LanguageDefinition> GetLanguages()
        {
            return LanguageDefinition.Definitions;
        }
    }
}
