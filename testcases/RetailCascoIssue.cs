using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace IMSAutomation.testcases
{
    [TestFixture]
    internal class RetailCascoIssue : BaseUITest

    {
        IPage page;
        public RetailCascoIssue(IPage page)
        {
            this.page = page;
        }


        [Test]
        public async Task RetailCascoIssuePhysicalPerson()
        {
            

        }
    }
}
