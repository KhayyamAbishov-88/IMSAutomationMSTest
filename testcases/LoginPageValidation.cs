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
        private const string ConnectionString = "Server=testserver01;Database=Eagle;User Id=sa_eagle;Password=Pony3201;TrustServerCertificate=True;";
        private const string UserLogin = "5-5-5-15";
        private const string UserPassword = "Pasyolka88";

       // [Test]
      /*  public async Task LoginWithValidCredentials ()
        {
            // using var playwright = await Playwright.CreateAsync();
            var (browser, page) = await CreateBrowserAndPage( playwright, "chrome", new BrowserTypeLaunchOptions { Headless = false } );

            var loginPage = new LoginPage( page );
            var afterLoginPage = await loginPage.LoginCredentials( UserLogin, UserPassword );
            if ( afterLoginPage is HomePage homePage )
            {
                await homePage.ClickProducts( new ProductsPage( page ) );
            }
            else
            {
                OtpPage otpPage = new OtpPage( page );
                otpPage.ClickToLoginViaOtp()
            }
            await page.WaitForLoadStateAsync( LoadState.NetworkIdle );
            //   await homePage.ClickProducts( new ProductsPage( page ) );

        }*/

        [Test]
        public async Task RedirectedToOtpPageSuccessfully ()
        {
            // using var playwright = await Playwright.CreateAsync();
            var (browser, page) = await CreateBrowserAndPage( playwright, "chrome", new BrowserTypeLaunchOptions { Headless = false } );
            var dbHelper = new DatabaseHelper();
            var (otpEnabled, otpSkipHours) = dbHelper.GetUserOtpPermission( UserLogin, ConnectionString );
            DateTime? optFistLoginDate = dbHelper.GetLastLoginDate( UserLogin, ConnectionString );
            var loginPage = new LoginPage( page );
            var afterLoginPage = await loginPage.LoginCredentials( UserLogin, UserPassword );

            bool shouldRequireOtp = otpEnabled && (
            otpSkipHours == null ||
            ( optFistLoginDate.HasValue &&
            optFistLoginDate.Value.Add( otpSkipHours.Value ) < DateTime.Now ) );

            if ( shouldRequireOtp )
            {
                // NUnit: object type is OtpPage
                Assert.That( afterLoginPage, Is.InstanceOf<OtpPage>(), "OTP is required, so OtpPage should load." );
                // OTP is required → OTP page should load
                await Assertions.Expect( page )
                        .ToHaveURLAsync( new Regex( "WebIMS/Account/GenerateOTP" ) );


            }
            else
            {
                // NUnit: object type is HomePage
                Assert.That( afterLoginPage, Is.InstanceOf<HomePage>(), "OTP not required, should land on HomePage." );
                await Assertions.Expect( page )
              .ToHaveURLAsync( new Regex( "/WebIMS/" ) );
            }


           
        }

        [Test]
        public async Task OtpSmsSendedSuccessfully ()
        {

            // using var playwright = await Playwright.CreateAsync();
            var (browser, page) = await CreateBrowserAndPage( playwright, "chrome", new BrowserTypeLaunchOptions { Headless = false } );
            var dbHelper = new DatabaseHelper();
           var (otp, smsSent) = dbHelper.GetLatestOtpCode( UserLogin, ConnectionString );


            if ( !smsSent )
            {
                Assert.Fail( "SMS was not sent for this user." );
            }

        }






    }
}