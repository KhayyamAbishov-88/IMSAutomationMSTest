using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSAutomation.pages;
using Microsoft.Playwright;
namespace IMSAutomation.Pages
{
    internal class LoginPage : BasePage
    {
        public LoginPage(IPage page) : base(page)
        {
           
        }

        public async Task<HomePage> LoginCredentials( string username, string password )
        {
            await page.Locator( "//*[@id=\"UserName\"]" ).FillAsync(username);
            await page.Locator( "//*[@id=\"Password\"]" ).FillAsync(password);
            var loginButton = page.Locator( "#LoginBtn" );

            // Wait until the button is visible and enabled
            await loginButton.WaitForAsync( new() { State = WaitForSelectorState.Visible } );

            // Click it
            await loginButton.ClickAsync();


            return new HomePage(page);
        }
    }
}
