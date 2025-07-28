using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Azure;
using IMSAutomation.pages;
using IMSAutomation.Pages;
using Microsoft.Playwright;
using IMSAutomation.utilities;
    
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

            RetailCascoPage retailCascoPage = new RetailCascoPage( page );
            await retailCascoPage.ClikcFillInputAsync( "#Vehicle_RegNr", RetailCascoPage.GetRandomVehicleRegNr() );

            await retailCascoPage.ClikcFillInputAsync( "#Vehicle_RegCertNumber", RetailCascoPage.GetRandomVehicleCerNr() );
          
            DatabaseHelper dbHelper = new DatabaseHelper(); 

            var vehicle = dbHelper.GetRandomVehicleFromView( "Server=test5;Database=Eagle;User Id=sa_eagle;Password=Pony3201;TrustServerCertificate=True;" );

            

            // 1. Select Brand
            await page.WaitForSelectorAsync( "#Vehicle_VehBrandOidText" );
            await retailCascoPage.SelectDropdownItem( "#Vehicle_VehBrandOidText", vehicle.BrandName );

            // 2. Select Model
            await page.WaitForSelectorAsync( "#Vehicle_VehModelOidText" );
            await retailCascoPage.SelectDropdownItem( "#Vehicle_VehModelOidText", vehicle.ModelName );

            // 3. Select Submodel
            await page.WaitForSelectorAsync( "#Vehicle_VehSubModelNameText" );
            await retailCascoPage.SelectDropdownItem( "#Vehicle_VehSubModelNameText", vehicle.SubModel );

            // 4. Select Year (age) — assuming Year dropdown is visible and interactive
            

            await page.WaitForSelectorAsync( "#Vehicle_ManufactoryYear" );
            await page.FillAsync( "#Vehicle_ManufactoryYear", ( DateTime.Now.Year - vehicle.YearOld ).ToString() );

            

        }

       

    }
}
