using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSAutomation.pages;
using IMSAutomation.Pages;
using Microsoft.Playwright;
using IMSAutomation.utilities;
using System.Text.RegularExpressions;
namespace IMSAutomation.testcases
{
    [TestFixture]
    internal class LoginPageValidation : BaseUITest
    {
        private const string ConnectionString = "Server=testserver01;Database=Eagle;User Id=sa_eagle;Password=Pony3201;TrustServerCertificate=True;";
        private const string UserLogin = "5-5-5-15";

       /* [Test]
        public async Task LoginWithValidCredentials ()
        {
            // using var playwright = await Playwright.CreateAsync();
            var (browser, page) = await CreateBrowserAndPage( playwright, "chrome", new BrowserTypeLaunchOptions { Headless = false } );
            var dbHelper = new DatabaseHelper();
            var (enabled, skipUntil) = dbHelper.GetUserOtpPermission( UserLogin, ConnectionString );
            LoginPage loginPage = new LoginPage( page );
            HomePage homePage = await loginPage.LoginCredentials( "5-5-5-15", "Pasyolka88" );
            await page.WaitForLoadStateAsync( LoadState.NetworkIdle );
            //   await homePage.ClickProducts( new ProductsPage( page ) );

        }*/

        [Test]
        public async Task LoginWithOtp ()
        {
            // using var playwright = await Playwright.CreateAsync();
            var (browser, page) = await CreateBrowserAndPage( playwright, "chrome", new BrowserTypeLaunchOptions { Headless = false } );
            var dbHelper = new DatabaseHelper();
            var (otpEnabled, otpSkipHours) = dbHelper.GetUserOtpPermission( UserLogin, ConnectionString );
            DateTime? optFistLoginDate = dbHelper.GetLastLoginDate( UserLogin, ConnectionString );
            var loginPage = new LoginPage( page );


            if ( otpEnabled &&   ( otpSkipHours == null || optFistLoginDate + otpSkipHours < DateTime.Now ))
            {
                await loginPage.LoginCredentials( UserLogin, "Pasyolka88" );
                // OTP is required → OTP page should load
                await Assertions.Expect( page )
                    .ToHaveURLAsync( new Regex( "WebIMS/Account/GenerateOTP" ) );
               
            }
            else
            {
                await loginPage.LoginCredentials( UserLogin, "Pasyolka88" );
                await Assertions.Expect( page )
              .ToHaveURLAsync( new Regex( "/WebIMS/" ) );
            }
        }
    }
}