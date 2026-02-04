using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
using IMSAutomation.Exceptions;
using System.Xml.Linq;
using RazorEngine;
using Microsoft.CodeAnalysis;

namespace IMSAutomation.Pages
{
    internal class LoginPage : BasePage
    {

        public LoginPage ( IPage page ) : base( page )
        {

        }



        public async Task<BasePage> RedirectPageAfterLogin ( string username, string password )
        {
            await page.Locator( "//*[@id=\"UserName\"]" ).FillAsync( username );
            await page.Locator( "//*[@id=\"Password\"]" ).FillAsync( password );
            var loginButton = page.Locator( "#LoginBtn" );

            // Wait until the button is visible and enabled
            await loginButton.WaitForAsync( new() { State = WaitForSelectorState.Visible } );

            // Click it
            await loginButton.ClickAsync();

            //click timestamp to get last opt code

            await page.WaitForLoadStateAsync( LoadState.NetworkIdle );
            // or
            await page.WaitForLoadStateAsync( LoadState.DOMContentLoaded );

            //try
            //{
            //    await page.Locator( "form[action$='/Account/VerifyOTP'], a[href$='/Account/Logout' i]" )
            //        .First
            //        .WaitForAsync( new() { State = WaitForSelectorState.Visible, Timeout = 30000 } );
            //}
            //catch ( TimeoutException )
            //{
            //    throw new Exception( $"Neither OTP form nor logout link appeared after login. Current URL: {page.Url}" );
            //}

            // After login button click
            if ( await page.Locator( "form[action$='/Account/VerifyOTP']" ).IsVisibleAsync() )
            {
                // OTP challenge page is active
                return ( new OtpPage( page ) );
            }
            else if ( await page.Locator( "a[href$='/Account/Logout' i]" ).IsVisibleAsync() )
            {
                // Logged in directly (home page, no OTP)
                return ( new HomePage( page ) );
            }
            else
            {
                throw new Exception( "Unexpected page state after login attempt." );
            }

        }
        public async Task EnterUsernameAsync ( string username )
        {
            await page.Locator( "#UserName" ).FillAsync( username );
        }

        public async Task EnterPasswordAsync ( string password )
        {
            await page.Locator( "#Password" ).FillAsync( password );
        }

        public async Task ClickLoginButtonAsync ()
        {
            await page.Locator( "#LoginBtn" ).ClickAsync();
        }

        public async Task LoginAsync ( string username, string password )
        {
            await EnterUsernameAsync( username );
            await EnterPasswordAsync( password );

            await ClickLoginButtonAsync();
        }


        /// <summary>
        /// Checks if a visible validation error message exists.
        /// Returns true only if the <li> is visible and has non-empty text.
        /// </summary>

        public async Task<bool> HasLoginValidationErrorAsync ()
        {
            var validationError = page.Locator( "div[data-valmsg-summary='true'] li" );

            if ( await validationError.CountAsync() == 0 )
                return false;

            bool isVisible = await validationError.IsVisibleAsync();
            string text = ( await validationError.TextContentAsync() )?.Trim();

            return isVisible && !string.IsNullOrEmpty( text );
        }


        /// <summary>
        /// Returns the text of the validation error (empty string if none).
        /// </summary>
        public async Task<string> GetLoginValidationErrorTextAsync ()
        {
            var validationError = page.Locator( "div[data-valmsg-summary='true'] li" );
            if ( await validationError.CountAsync() == 0 )
                return string.Empty;

            string text = ( await validationError.TextContentAsync() )?.Trim();
            return text ?? string.Empty;
        }

        public async Task<string> GetDeviceFingerprintAsync ()
        {
            var deviceFingerprintInput = page.Locator( "#DeviceFingerprint" );
            await deviceFingerprintInput.WaitForAsync( new() { State = WaitForSelectorState.Attached } );

            return await deviceFingerprintInput.InputValueAsync();

        }

        public async Task LogoutAsync() 
        { 
            var logoutLink =  page.Locator( "a[href='/WebIMS/Account/Logout']" );
            await logoutLink.WaitForAsync( new() { State = WaitForSelectorState.Visible } );
            await logoutLink.ClickAsync();
        }

    }
}



