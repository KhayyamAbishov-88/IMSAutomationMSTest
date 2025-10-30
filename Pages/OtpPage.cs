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

        public async Task<HomePage> ClickToLoginViaOtp ( string otpCode )
        {
            await page.Locator( "#OTPCode" ).FillAsync( otpCode );
            var verifyButton = page.Locator( "input[name='VerifyOTP']" );
            await verifyButton.ClickAsync();



            return new HomePage( page );
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

