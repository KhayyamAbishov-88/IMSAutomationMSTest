using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Playwright;
namespace IMSAutomation.Pages
{
    internal class LoginPage : BasePage
    {
        public LoginPage(IPage page) : base(page)
        {
           
        }

        public async Task<BasePage> LoginCredentials( string username, string password )
        {
            await page.Locator( "//*[@id=\"UserName\"]" ).FillAsync(username);
            await page.Locator( "//*[@id=\"Password\"]" ).FillAsync(password);
            var loginButton = page.Locator( "#LoginBtn" );

            // Wait until the button is visible and enabled
            await loginButton.WaitForAsync( new() { State = WaitForSelectorState.Visible } );
           
            // Click it
            await loginButton.ClickAsync();

            await page.WaitForLoadStateAsync( LoadState.NetworkIdle );
            // or
            await page.WaitForLoadStateAsync( LoadState.DOMContentLoaded );

            // After login button click
            if ( await page.Locator( "form[action='/WebIMS/Account/VerifyOTP']" ).IsVisibleAsync() )
            {
                // OTP challenge page is active
                return new OtpPage( page );
            }
            else
            {
                // Logged in directly (home page, no OTP)
                return new HomePage( page );
            }

           
        }
    }
}
