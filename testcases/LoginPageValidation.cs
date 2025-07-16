using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace IMSAutomation.testcases
{
    [TestFixture]
    internal class LoginPageValidation :BaseUITest
    {
        IPage page;
        public LoginPageValidation(IPage page)
        {
            this.page = page;
        }

        [SetUp]
        public async Task Setup()
        {
            await page.GotoAsync("https://ims.qarabag.az/");
            await page.Locator("//*[@id=\"UserName\"]").FillAsync("testuser");
            await page.Locator("//*[@id=\"Password\"]").FillAsync("testpassword");
            await page.Locator("//*[@id=\"LoginBtn\"]").ClickAsync();
        }

    {
       


        [Test, Order(1), Description("Login with valid credentials")]
        public async Task LogoinWithValidCredentials ()
        {
            

        }


    }
}
