using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace IMSAutomation.testcases
{
    [TestFixture]
    internal class LoginPageValidation
    {
        [SetUp]
        public async Task Setup()
        {
            using var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync( new BrowserTypeLaunchOptions
            {
                Headless = false

            } );
            var page = await browser.NewPageAsync();
            await page.GotoAsync( "https://testserver01-polis.ateshgah.com/WebIMS/Account/Login" );
        }

        public async Task LogoinWithValidCredentials ()
        {
            

        }


    }
}
