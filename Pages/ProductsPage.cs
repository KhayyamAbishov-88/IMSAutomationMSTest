using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSAutomation.Pages;
using Microsoft.Playwright;

namespace IMSAutomation.testcases
{
    internal class ProductsPage : BasePage
    {
        public ProductsPage ( IPage page ) : base( page )
        {

        }


        public async Task<RetailCascoPage> GoToReatilCascoEditView()
        {
            await page.GetByRole( AriaRole.Link, new() { Name = "Məhsullar" } ).HoverAsync();
            await page.GetByRole( AriaRole.Link, new() { Name = "Agent Kaskosu" } ).ClickAsync();
            return new RetailCascoPage(page);
        }


    }
}
    