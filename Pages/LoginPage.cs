using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
namespace IMSAutomation.Pages
{
    internal class LoginPage
    {
        IPage page;
        public LoginPage( IPage page )
        {
            this.page = page;
        }

        public async Task LoginCredentials( string username, string password )
        {
            await page.Locator( "//*[@id=\"UserName\"]" ).FillAsync(username);
            await page.Locator( "//*[@id=\"Password\"]" ).FillAsync(password);
            await page.Locator( "//*[@id=\"LoginBtn\"]" ).ClickAsync();
        }
    }
}
