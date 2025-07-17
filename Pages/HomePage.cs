using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSAutomation.Pages;
using IMSAutomation.testcases;
using Microsoft.Playwright;

namespace IMSAutomation.pages

{
    internal class HomePage : BasePage

    {
        public HomePage( IPage page ) : base( page )
        {

        }

        public async Task ClickProducts(ProductsPage productsPage)
        {
            await productsPage.GoToReatilCascoEditView();
            await page.WaitForLoadStateAsync( LoadState.NetworkIdle );

        }
    }
}
