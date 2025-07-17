using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSAutomation.Pages;
using Microsoft.Playwright;

namespace IMSAutomation.testcases
{
    [TestFixture]
    internal class LoginPageValidation : BaseUITest
    {
        IPage page;
        public LoginPageValidation ( IPage page )
        {
            this.page = page;
        }

        [Test]
        public async Task LoginWithValidCredentials ()
        {
            LoginPage loginPage = new LoginPage(page);  
        }
    }
}