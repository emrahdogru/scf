using Scf.Controllers;
using Scf.Domain;
using Scf.Domain.Services;
using Scf.IntegrationTest.FakeServices;
using Scf.IntegrationTest.Helpers;
using Scf.LanguageResources;
using Scf.Models.Forms;
using Scf.Models.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Data;
using System.Linq.Dynamic.Core.Tokenizer;

namespace Scf.IntegrationTest
{
    [Collection(nameof(SignletonServiceFixture))]
    public class PasswordControllerTest
    {
        readonly SignletonServiceFixture Fixture;

        public PasswordControllerTest(SignletonServiceFixture fixture)
        {
            this.Fixture = fixture;
        }

        [Fact]
        public async Task Change_Success()
        {
            using var scope = Fixture.ServiceProvider.CreateScope();
            var helper = new Helper(scope);

            var user = helper.UserHelper.CreateRandomUser(out string password);
            helper.SetAuthenticationToken(user);

            
            var passwordController = ActivatorUtilities.CreateInstance<PasswordController>(scope.ServiceProvider);

            string newPassword = "Test-963852";

            var passwordChangeForm = new ChangePasswordForm()
            {
                OldPassword = password,
                NewPassword = newPassword,
                ConfirmPassword = newPassword,
            };


            try
            {
                await passwordController.Change(passwordChangeForm);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }


            if (!user.IsValidPassword(newPassword))
                Assert.Fail("Parola değişmemiş.");
        }

        [Fact]
        public async Task Change_Fail_OldPasswordInvalid()
        {
            using var scope = Fixture.ServiceProvider.CreateScope();
            var helper = new Helper(scope);

            var user = helper.UserHelper.CreateRandomUser(out string password);
            helper.SetAuthenticationToken(user);
            
            var passwordController = ActivatorUtilities.CreateInstance<PasswordController>(scope.ServiceProvider);

            string newPassword = Guid.NewGuid().ToString();
            var form = new ChangePasswordForm()
            {
                OldPassword = "hatalı parola",
                NewPassword = newPassword,
                ConfirmPassword = newPassword
            };

            try
            {
                await passwordController.Change(form);
                Assert.Fail("Eski parola hatalı olduğu halde parola değiştirildi.");
            }
            catch (UserException ex)
            {
                var l = scope.ServiceProvider.GetService<ILanguageService>();
                Assert.Equal(ex.Message, l.Get(x => x.FieldInvalid, new { field = l.Get(x => x.Password) }));
            }
        }

        [Fact]
        public async Task Change_Fail_Validation()
        {
            using var scope = Fixture.ServiceProvider.CreateScope();
            var helper = new Helper(scope);
            var l = scope.ServiceProvider.GetService<ILanguageService>();

            var user = helper.UserHelper.CreateRandomUser(out string password);
            helper.SetAuthenticationToken(user);

            var passwordController = ActivatorUtilities.CreateInstance<PasswordController>(scope.ServiceProvider);

            var form = new ChangePasswordForm()
            {
                OldPassword = password,
                NewPassword = Guid.NewGuid().ToString(),
                ConfirmPassword = Guid.NewGuid().ToString()
            };

            try
            {
                await passwordController.Change(form);
                Assert.Fail("Doğrulama parolası hatalı olduğu halde değiştirildi.");
            }
            catch (FluentValidation.ValidationException ex)
            {
                var expectedErrorMessage = l.Get(x => x.FieldInvalid, new { field = l.Get(x => x.ConfirmPassword) });
                Assert.Contains(expectedErrorMessage, ex.Errors.Select(x => x.ErrorMessage));
            }
        }
    }
}