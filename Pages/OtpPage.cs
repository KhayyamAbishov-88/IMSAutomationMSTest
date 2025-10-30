using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Playwright;

namespace IMSAutomation.Pages
{
    internal class OtpPage : BasePage
    {

        public OtpPage ( IPage page ) : base( page )
        {

        }

        public async Task<HomePage> RedirectToHomePageAfterOpt ( string otpCode )
        {
            await page.Locator( "#OTPCode" ).FillAsync( otpCode );
            var verifyButton = page.Locator( "input[name='VerifyOTP']" );
            await verifyButton.ClickAsync();

            await page.WaitForLoadStateAsync( LoadState.NetworkIdle );
            // or
            await page.WaitForLoadStateAsync( LoadState.DOMContentLoaded );

            if ( await page.Locator( "a[href$='/Account/Logout' i]" ).IsVisibleAsync() )
            {
                // Logged in  (home page, with OTP)
                return ( new HomePage( page ) );
            }
            else
            {
                throw new Exception( "Unexpected page state after login attempt." );
            }

           
        }

        public async Task EnterOtpCode ( string otpCode )
        {
            await page.Locator( "#OTPCode" ).FillAsync( otpCode );
        }

        public async Task ClickSubmitOtpButton ()
        {
            var verifyButton = page.Locator( "input[name='VerifyOTP']" );
            await verifyButton.ClickAsync();
        }

        public async Task<bool> HasOtpAlreadySendValidationErrorAsync ()
        {
            var validationError = page.Locator( "#PageMessageBox>p.error" );

            await page.WaitForSelectorAsync( "div#PageMessageBox p.error", new() { State = WaitForSelectorState.Visible } );
            if ( await validationError.CountAsync() == 0 )
                return false;

            bool isVisible = await validationError.IsVisibleAsync();
            string text = ( await validationError.TextContentAsync() )?.Trim();

            return isVisible && !string.IsNullOrEmpty( text );
        }

        public async Task<string> GetOtpAlreadySendValidationErrorTextAsync ()
        {
            var validationError = page.Locator( "#PageMessageBox>p.error" );

            if ( await validationError.CountAsync() == 0 )
                return string.Empty;

            return ( await validationError.TextContentAsync() )?.Trim() ?? string.Empty;
        }

        public async Task CloseAsync()
        {
            await page.CloseAsync();
        }



    }
}

