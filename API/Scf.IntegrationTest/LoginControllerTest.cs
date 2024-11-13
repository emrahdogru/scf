using Scf.Controllers;
using Scf.Domain;
using Scf.Domain.Services;
using Scf.IntegrationTest.FakeServices;
using Scf.Models.Forms;
using Scf.Models.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Scf.IntegrationTest
{
    [Collection(nameof(SignletonServiceFixture))]
    public class LoginControllerTest
    {
        readonly SignletonServiceFixture Fixture;

        public LoginControllerTest(SignletonServiceFixture fixture)
        {
            this.Fixture = fixture;
        }

        [Fact]
        public async Task Post_Success()
        {
            using var scope = this.Fixture.ServiceProvider.CreateScope();

            var domainContext = scope.ServiceProvider.GetService<DomainContext>() ?? throw new NullReferenceException();
            var tokenService = scope.ServiceProvider.GetService<ITokenService>();

            var loginController = ActivatorUtilities.GetServiceOrCreateInstance<LoginController>(scope.ServiceProvider);


            var user = new Helper(scope).UserHelper.CreateRandomUser(out string password);

            var loginForm = new LoginForm() { Email = user.Email, Password = password };

            TokenResult? result = null;

            try
            {
                result = await loginController.Post(loginForm);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

            Assert.NotNull(result);
            Assert.NotNull(result.Key);
            Assert.Equal(user.Id, result.User.Id);

            var token = await tokenService.Parse(result.Key);

            Assert.NotNull(token);
            Assert.True(token.User == user, "Oturumu açılmak istenen kullanıcı ile oturumu açılan kullanıcı aynı değil.");
        }

        [Fact]
        public async Task Post_Fail_UserNotExist()
        {

            using var scope = this.Fixture.ServiceProvider.CreateScope();
            var l = ActivatorUtilities.GetServiceOrCreateInstance<ILanguageService>(scope.ServiceProvider);
            var loginController = ActivatorUtilities.CreateInstance<LoginController>(scope.ServiceProvider);

            var form = new LoginForm()
            {
                Email = $"{Guid.NewGuid():N}@example.com",
                Password = "Test-1234"
            };

            TokenResult result;

            try
            {
                result = await loginController.Post(form);
                Assert.Fail("Bu kullanıcı olmadığı için oturum açmanın başarısız olması gerekirdi.");
            }
            catch (UserException ex)
            {
                Assert.Equal(l.Get(x => x.LoginFailure), ex.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        public async Task Post_Fail_InvalidPassword()
        {
            using var scope = Fixture.ServiceProvider.CreateScope();
            var l = scope.ServiceProvider.GetService<ILanguageService>();

            var user = new Helper(scope).UserHelper.CreateRandomUser(out string password);

            var loginController = ActivatorUtilities.CreateInstance<LoginController>(scope.ServiceProvider);

            var form = new LoginForm()
            {
                Email = user.Email,
                Password = Guid.NewGuid().ToString()
            };

            TokenResult result;

            try
            {
                result = await loginController.Post(form);
                Assert.Fail("Parola geçersiz olduğu için oturum açmanın başarısız olması gerekirdi.");
            }
            catch (UserException ex)
            {
                Assert.Equal(l.Get(x => x.LoginFailure), ex.Message);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        public void Get_Success()
        {
            using var scope = Fixture.ServiceProvider.CreateScope();
            var helper = new Helper(scope);

            var user = Fixture.AbsoluteUser;
            var token = Fixture.AbsoluteUserToken;

            helper.SetAuthenticationToken(token.Key);

            var loginController = ActivatorUtilities.CreateInstance<LoginController>(scope.ServiceProvider);
            var tokenResult = loginController.Get();

            Assert.Equal(token.Key, tokenResult.Key);
        }

        [Fact]
        public void Get_Fail_InvalidToken()
        {
            using var scope = Fixture.ServiceProvider.CreateScope();

            var httpContext = scope.ServiceProvider.GetService<IHttpContextService>() as FakeHttpContextService;
            httpContext.Token = $"{Guid.NewGuid():N}{Guid.NewGuid():N}";

            var loginController = ActivatorUtilities.CreateInstance<LoginController>(scope.ServiceProvider);

            try
            {
                var tokenResult = loginController.Get();
                Assert.Fail("Geçersiz token gönderildiği için başarısız olması gerekirdi.");
            }
            catch (InvalidTokenException)
            { }
            catch (AggregateException ae)
            {
                if (!ae.InnerExceptions.Any(x => x is InvalidTokenException))
                    Assert.Fail("InvalidTokenException bekliyorduk, gelmedi.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        public void Get_Fail_EmptyToken()
        {
            using var scope = Fixture.ServiceProvider.CreateScope();
            var helper = new Helper(scope);


            helper.SetAuthenticationToken(string.Empty);

            var loginController = ActivatorUtilities.CreateInstance<LoginController>(scope.ServiceProvider);

            try
            {
                var tokenResult = loginController.Get();
                Assert.Fail("Geçersiz token gönderildiği için başarısız olması gerekirdi.");
            }
            catch (InvalidTokenException)
            { }
            catch (AggregateException ae)
            {
                if (!ae.InnerExceptions.Any(x => x is InvalidTokenException))
                    Assert.Fail("InvalidTokenException bekliyorduk, gelmedi.");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}