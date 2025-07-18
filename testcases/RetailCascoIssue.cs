using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using IMSAutomation.pages;
using IMSAutomation.Pages;
using Microsoft.Playwright;

namespace IMSAutomation.testcases
{
    [TestFixture]
    internal class RetailCascoIssue : BaseUITest

    {


        [Test]
        public async Task RetailCascoIssuePhysicalPerson()
        {
           
            var (browser, page) = await CreateBrowserAndPage(playwright, "chrome", new BrowserTypeLaunchOptions { Headless = false });

            LoginPage loginPage = new LoginPage( page );
            HomePage homePage = await loginPage.LoginCredentials( "1-1-2-15", "Aa123456789" );
            await homePage.ClickProducts( new ProductsPage( page ) );



        }
    }
}
