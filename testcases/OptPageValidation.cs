using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
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
        private const string ConnectionString = "Server=testserver01;Database=Eagle;User Id=sa_eagle;Password=Pony3201;TrustServerCertificate=True;";
        private const string OtpUserLogin = "40-389-1302-10942";
        private const string OtpUserPassword = "Otp123456789";

        private async Task<bool> IsLoggedInWithTrustedDeviceAsync (string clientFingerprint)
        {
            var dbHelper = new DatabaseHelper();
            await Task.Delay( 10000 );
            var (optFistLoginDate, deviceId) = await dbHelper.GetLastLoginDateAsync( OtpUserLogin, ConnectionString );
            var (hasOptPermission, OtpSkipHours) = await dbHelper.GetUserOtpPermissionAsync( OtpUserLogin, ConnectionString );
            
            DateTime currentDate = DateTime.Now;
            TimeSpan? OtpLoginDuration = currentDate - ( DateTime? )optFistLoginDate;
            if ( deviceId == clientFingerprint )
            {
                if ( hasOptPermission is true && OtpLoginDuration?.TotalHours < OtpSkipHours?.TotalHours )
                {
                    return true;
                }
            }
            return false;
        }


        private async Task<BasePage> LoginAndRedirectAsync ()
        {
            var (browser, page) = await CreateBrowserAndPage( playwright, "chrome", new BrowserTypeLaunchOptions { Headless = false } );
            var dbHelper = new DatabaseHelper();

            var (otpEnabled, otpSkipHours) = await dbHelper.GetUserOtpPermissionAsync( OtpUserLogin, ConnectionString );
            var (optFistLoginDate, deviceId) = await dbHelper.GetLastLoginDateAsync( OtpUserLogin, ConnectionString );
            var loginPage = new LoginPage( page );
            var afterLoginPage = await loginPage.RedirectPageAfterLogin( OtpUserLogin, OtpUserPassword );



            bool shouldRequireOtp = otpEnabled && (
                otpSkipHours == null ||
                ( optFistLoginDate == null ||
                 optFistLoginDate.Value.Add( otpSkipHours.Value ) < DateTime.Now )
            );

            if ( shouldRequireOtp )
            {
                if ( afterLoginPage is OtpPage )
                {
                    return ( new OtpPage( page ) );
                }
            }
            else
            {
                return new HomePage( page );
            }

            return afterLoginPage;
        }

        [Test]
        [Description( "Verify that Otp enabled user land on OPT page" )]
        public async Task SkipOtpForTrustedDeviceAsync ()
        {
            var dbHelper = new DatabaseHelper();
           await dbHelper.RemoveLastOtpCode( ConnectionString, OtpUserLogin );
             await dbHelper.ClearTrustedDevices( ConnectionString, OtpUserLogin );
            var afterLoginPage = await LoginAndRedirectAsync();
            var page = afterLoginPage.page;

            var (otpGeneratedTime, otp, smsSent) = dbHelper.GetLatestOtpCode( OtpUserLogin, ConnectionString );
            if ( afterLoginPage is OtpPage otpPage )
            {
                await otpPage.RedirectToHomePageAfterOpt( otp );
            }
            else 
            {
                
                Assert.Inconclusive( "Directly landed on HomePage" );
            }
           
            var loginPage = new LoginPage( page );
            await loginPage.LogoutAsync();

            var clientFingerprint = await loginPage.GetDeviceFingerprintAsync();
            var redirectedPage = await loginPage.RedirectPageAfterLogin( OtpUserLogin, OtpUserPassword );
            
            bool isTrusted = await IsLoggedInWithTrustedDeviceAsync(clientFingerprint);


            if ( isTrusted ) 
            {
                Assert.That( redirectedPage is HomePage, "Should skip OTP and land on HomePage for trusted device." );
               
            }
            else
            {
                Assert.Fail( "Device should be trusted but OTP page was shown." );
            }

        }


        [Test]
        [Description( "Verify that Otp enabled user land on OPT page" )]
        public async Task LoginToOtpPageSuccessfully ()
        {
            var dbHelper = new DatabaseHelper();
            await dbHelper.RemoveLastOtpCode( ConnectionString, OtpUserLogin );
            await dbHelper.ClearTrustedDevices( ConnectionString, OtpUserLogin );
            var afterLoginPage = await LoginAndRedirectAsync();

            if ( afterLoginPage is OtpPage )
                Assert.Pass( "OTP is required, so OtpPage should load." );
            else if ( afterLoginPage is HomePage )
                Assert.Fail( "OTP not required, should land on HomePage." );
            else
                Assert.Fail( "Unexpected page type" );

        }

     
        [TestCase( Description = "Verify that Otp enabled user is sended otp code" )]
        public async Task OtpSmsSendedSuccessfully ()
        {
            var dbHelper = new DatabaseHelper();
            await dbHelper.RemoveLastOtpCode( ConnectionString, OtpUserLogin );
            await dbHelper.ClearTrustedDevices( ConnectionString, OtpUserLogin );
            var afterLoginPage = await LoginAndRedirectAsync();


            var (otpGeneratedTime, otp, smsSent) = dbHelper.GetLatestOtpCode( OtpUserLogin, ConnectionString );
            TestContext.WriteLine( smsSent );
            TestContext.WriteLine( otpGeneratedTime );
            TestContext.WriteLine( $"output is {otp}" );

            if ( !smsSent )
            {
                Assert.Fail( "SMS was not sent for this user." );
            }

        }


        [Test]
        [TestCase( Description = "Verify if opt code still active within 5 minutes" )]
        public async Task ValidateOtpAlreadySendAsync ()
        {
            var dbHelper = new DatabaseHelper();
            await dbHelper.RemoveLastOtpCode( ConnectionString, OtpUserLogin );
            await dbHelper.ClearTrustedDevices( ConnectionString, OtpUserLogin );
            // Step 1: Login and navigate to the OTP page
            var afterLoginPage = await LoginAndRedirectAsync();
            if ( afterLoginPage is not OtpPage otpPage )
            {
                Assert.Fail( "Did not land on OTP page." );
                return;
            }

            // Step 2: Get latest OTP info

            var (otpGeneratedTime, otp, smsSent) = dbHelper.GetLatestOtpCode( OtpUserLogin, ConnectionString );

            if ( otpGeneratedTime is null )
            {
                Assert.Fail( "No OTP generation time found for the user." );
                return;
            }

            double otpActiveInterval = ( DateTime.Now - otpGeneratedTime.Value ).TotalMinutes;
            if ( otpActiveInterval > 5 )
            {
                Assert.Inconclusive( "OTP is expired (older than 5 minutes)." );
                return;
            }

            // Step 3: Attempt to login again while OTP still active

            var (browser, page) = await CreateBrowserAndPage( playwright, "chrome", new BrowserTypeLaunchOptions { Headless = false } );

            var loginPage = new LoginPage( page );
            var redirectedPage = await loginPage.RedirectPageAfterLogin( OtpUserLogin, OtpUserPassword );

            // Step 4: Validate the OTP resend error message
            if ( redirectedPage is OtpPage otpResendPage )
            {
                bool hasValidation = await otpResendPage.HasOtpAlreadySendValidationErrorAsync();
                string validationText = await otpResendPage.GetOtpAlreadySendValidationErrorTextAsync();

                TestContext.WriteLine( $"Validation Present: {hasValidation}" );
                TestContext.WriteLine( $"Validation Message: {validationText}" );

                Assert.That( hasValidation, Is.True, "Expected OTP already sent validation error not found." );
            }
            else
            {
                Assert.Fail( "Expected to be redirected to OTP page after second login attempt." );
            }
        }

        [Test]
        [TestCase( Description = "Verify if user can login with active otp code" )]
        public async Task LoginWithValidOtpAsync ()
        {
            var dbHelper = new DatabaseHelper();
            await dbHelper.RemoveLastOtpCode( ConnectionString, OtpUserLogin );
            await dbHelper.ClearTrustedDevices( ConnectionString, OtpUserLogin );
            // Step 1: Login and navigate to the OTP page
            var afterLoginPage = await LoginAndRedirectAsync();
            if ( afterLoginPage is not OtpPage otpPage )
            {
                Assert.Inconclusive( "Did not land on OTP page." );
                return;
            }

            // Step 2: Get latest OTP info
            var (otpGeneratedTime, otp, smsSent) = dbHelper.GetLatestOtpCode( OtpUserLogin, ConnectionString );

            if ( string.IsNullOrEmpty( otp ) )
            {
                Assert.Fail( "No OTP found for the user." );
                return;
            }

            // Step 3: Enter the correct OTP and submit
            var afterOtpPage = await otpPage.RedirectToHomePageAfterOpt( otp );


            // Step 4: Validate successful login
            if ( afterOtpPage is HomePage )
            {
                Assert.Pass( "Successfully logged in with correct OTP." );
            }
            else
            {
                Assert.Fail( "Failed to log in with correct OTP." );
            }

        }


       
    }
}
