using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSAutomation.TestCases;
using Microsoft.Playwright;

namespace IMSAutomation.Pages
{
    internal class HomePage : BasePage

    {
        public HomePage ( IPage page ) : base( page )
        {

        }

        public async Task ClickProducts ( ProductsPage productsPage )
        {
            await productsPage.GoToReatilCascoEditView();




        }

        public async Task LogoutAsync ()
        {
            var logoutLink = page.Locator( "a[href='/WebIMS/Account/Logout']" );
            await logoutLink.WaitForAsync( new() { State = WaitForSelectorState.Visible } );
            await logoutLink.ClickAsync();
        }
    }
}
   

