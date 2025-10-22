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
            var browserAndPage = await CreateBrowserAndPage( playwright, "chrome", new BrowserTypeLaunchOptions { Headless = false } );


           IBrowser browser= browserAndPage.Item1;
           IPage page= browserAndPage.Item2;
            var loginPage = new LoginPage( page );
            await loginPage.LoginAsync( "444", "Pasyolka88" );



            var liLocator = page.Locator( "div[data-valmsg-summary='true'] li" );

            bool hasError = await loginPage.HasValidationErrorAsync();
            string errorText = await loginPage.GetValidationErrorTextAsync();

            TestContext.WriteLine( hasError );
            estContext.WriteLine( errorText );
            Assert.That( hasError, Is.True, "Validation message should not be visible initially." );
            Assert.That( errorText, Does.Contain( "The user name or password provided is incorrect." ) );




        }



    }
}
