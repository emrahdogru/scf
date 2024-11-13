using Scf.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scf.IntegrationTest
{
    [Collection(nameof(SignletonServiceFixture))]
    public class LanguageResourceControllerTest
    {
        readonly SignletonServiceFixture Fixture;

        public LanguageResourceControllerTest(SignletonServiceFixture fixture)
        {
            this.Fixture = fixture;
        }

        [Fact]
        public void Languages_Get_Success()
        {
            using var scope = Fixture.ServiceProvider.CreateScope();
            var controller = ActivatorUtilities.CreateInstance<LanguageResourceController>(scope.ServiceProvider);

            var languages = controller.GetLanguages();

            foreach (var l in Enum.GetValues<Languages>())
            {
                Assert.Contains(l, languages.Keys);
            }
        }
    }
}
