using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace IMSAutomation.Pages
{
    internal class BasePage
    {
        public IPage page;

        public BasePage ( IPage page )
        {
            this.page = page;
        }

    }
}


   

