using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IMSAutomation.Pages;
using Microsoft.Playwright;
using IMSAutomation.utilities;
using System.Text.RegularExpressions;
namespace IMSAutomation.TestCases
{
    [TestClass]
    public class LoginPageValidation : BaseUITest
    {
        private const string ConnectionString = "Server=test5;Database=Eagle;User Id=sa_eagle;Password=Pony3201;TrustServerCertificate=True;";
        private const string InvalidUserLogin = "5-5-5-17";
        private const string UserPassword = "Sinoptik88";
        private DateTime clickLoginDateTime;






        [TestMethod]
        public async Task Login_WithInvalidUsernameAsync()
        {
            var (browser, page) = await CreateBrowserAndPage(playwright, "chrome", new BrowserTypeLaunchOptions { Headless = false });


            var loginPage = new LoginPage(page);
            await loginPage.LoginAsync(InvalidUserLogin, UserPassword);
            bool hasValidation = await loginPage.HasLoginValidationErrorAsync();
            string valdationText = await loginPage.GetLoginValidationErrorTextAsync();
            TestContext.WriteLine(hasValidation.ToString());
            TestContext.WriteLine(valdationText);
            Assert.IsTrue(hasValidation, "Validation message should not be visible initially.");
            StringAssert.Contains(valdationText, "The user name or password provided is incorrect.");

        }



    }
}
