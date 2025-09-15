using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace IMSAutomation.Pages
{
    internal class ProductsPage : BasePage
    {
        public ProductsPage ( IPage page ) : base( page )
        {

        }


        public async Task<RetailCascoPage> GoToReatilCascoEditView ()
        {
            await page.Locator( "ul.menu-links > li" ).Nth( 0 ).HoverAsync();
            await page.Locator( "a[href='/WebIMS/Common/Policy/Create/43']" ).ClickAsync();
            // await page.WaitForSelectorAsync( "#Status" );

            await page.WaitForSelectorAsync( "#Status", new() { State = WaitForSelectorState.Visible } );
            await page.WaitForLoadStateAsync( LoadState.DOMContentLoaded );
            return new RetailCascoPage( page );
        }


    }
}

  

    