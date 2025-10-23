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
    [TestFixture, Parallelizable( ParallelScope.None )]
    internal class LoginPageValidation : BaseUITest
    {
        private const string ConnectionString = "Server=test5;Database=Eagle;User Id=sa_eagle;Password=Pony3201;TrustServerCertificate=True;";
        private const string UserLogin = "5-5-5-15";
        private const string UserPassword = "Sinoptik88";
        private DateTime clickLoginDateTime;






        [Test]
        public async Task Login_WithInvalidUsernameAsync ()
        {
            var (browser,page) = await CreateBrowserAndPage( playwright, "chrome", new BrowserTypeLaunchOptions { Headless = false } );


            var loginPage = new LoginPage( page );
            await loginPage.LoginAsync( "5-5-5-15", "Sinoptik88" );



           

            bool hasValidation = await loginPage.HasLoginValidationErrorAsync();
            string valdationText = await loginPage.GetLoginValidationErrorTextAsync();

            TestContext.WriteLine( hasValidation );
            TestContext.WriteLine( valdationText );
            Assert.That( hasValidation, Is.True, "Validation message should not be visible initially." );
            Assert.That( valdationText, Does.Contain( "The user name or password provided is incorrect." ) );

        }



    }
}
