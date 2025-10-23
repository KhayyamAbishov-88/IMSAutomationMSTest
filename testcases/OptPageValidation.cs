using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IMSAutomation.Pages;
using Microsoft.Playwright;
using IMSAutomation.utilities;
using System.Text.RegularExpressions;
using Azure;

namespace IMSAutomation.TestCases
{
    [TestFixture, Parallelizable( ParallelScope.None )]
    internal class OptPageValidation : BaseUITest
    {
        private const string ConnectionString = "Server=test5;Database=Eagle;User Id=sa_eagle;Password=Pony3201;TrustServerCertificate=True;";
        private const string OtpUserLogin = "5-5-5-15";
        private const string OtpUserPassword = "Sinoptik88";

        [Test, Order( 1 )]
        public async Task RedirectedToOtpPageSuccessfully ()
        {
            // using var playwright = await Playwright.CreateAsync();
            var (browser,page) = await CreateBrowserAndPage( playwright, "chrome", new BrowserTypeLaunchOptions { Headless = false } );
            var dbHelper = new DatabaseHelper();
            var (otpEnabled, otpSkipHours) = dbHelper.GetUserOtpPermission( OtpUserLogin, ConnectionString );
            DateTime? optFistLoginDate = dbHelper.GetLastLoginDate( OtpUserLogin, ConnectionString );
            var loginPage = new LoginPage( page );
            var afterLoginPage = await loginPage.RedirectPageAfterLogin( OtpUserLogin, OtpUserPassword );
             

            bool shouldRequireOtp = otpEnabled && (
            otpSkipHours == null ||
            ( optFistLoginDate.HasValue &&
            optFistLoginDate.Value.Add( otpSkipHours.Value ) < DateTime.Now ) );
            TestContext.WriteLine( shouldRequireOtp );

          
            if ( shouldRequireOtp && afterLoginPage is OtpPage otpPage )
            {
                    TestContext.WriteLine( afterLoginPage );
                    // NUnit: object type is OtpPage
                    //Assert.That( afterLoginPage, Is.InstanceOf<OtpPage>(), "OTP is required, so OtpPage should load." );
                    Assert.Pass( "OTP is required, so OtpPage should load." );
                

            }



            else if (  afterLoginPage is HomePage )
            {
                Assert.Fail( "OTP not required, should land on HomePage." );
              //  Assert.That( afterLoginPage, Is.InstanceOf<HomePage>(), "OTP not required, should land on HomePage." );

            }

            else
            {
                Assert.Fail("lll");
              
            }



        }

        [Test, Order( 2 )]
        public async Task OtpSmsSendedSuccessfully ()
        {


            // var (browser, page) = await CreateBrowserAndPage( playwright, "chrome", new BrowserTypeLaunchOptions { Headless = false } );
            var dbHelper = new DatabaseHelper();
          
            var (otp, smsSent) = dbHelper.GetLatestOtpCode( OtpUserLogin, ConnectionString);
            TestContext.WriteLine (smsSent);
            TestContext.WriteLine($"output is {otp}" );

            if ( !smsSent )
            {
                Assert.Fail( "SMS was not sent for this user." );
            }

        }




       





    }
}
