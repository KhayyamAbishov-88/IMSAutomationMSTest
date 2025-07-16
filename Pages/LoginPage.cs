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
            await page.Locator( "//*[@id=\"LoginBtn\"]" ).ClickAsync();

            return new HomePage(page);
        }
    }
}
