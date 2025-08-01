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
            await retailCascoPage.ClikcFillVechRegNumInputAsync( RetailCascoPage.GetRandomVehicleRegNr() );

            await retailCascoPage.ClikcFillVechCertNumInputAsync(  RetailCascoPage.GetRandomVehicleCerNr() );
          
            DatabaseHelper dbHelper = new DatabaseHelper(); 

            var vehicle = dbHelper.GetRandomVehicleFromView( "Server=testserver01;Database=Eagle;User Id=sa_eagle;Password=Pony3201;TrustServerCertificate=True;" );

            

            // 1. Select Brand
          
            
            await retailCascoPage.SelectVehicleBrand(vehicle.BrandName );

            // 2. Select Model
          
            await retailCascoPage.SelectVehicleModel( vehicle.ModelName );

            // 3. Select Submodel
           
            await retailCascoPage.SelectVehicleSubModel( vehicle.SubModel );

            // 4. Select Year (age) — assuming Year dropdown is visible and interactive
            

         
            await retailCascoPage.ClikcFillVechManfactYearInputAsync(( DateTime.Now.Year - vehicle.YearOld ).ToString() );

            await retailCascoPage.SelectVehicleUsage();
            await retailCascoPage.SelectDeducible();

            await retailCascoPage.ClickSectionByNameAsync(page,"Sığorta");

            await retailCascoPage.SearchPolicyHolderInfo( "4Z6NNQZ", "AA", "6095063" ,"994512068475");

            await retailCascoPage.ClickToCalculatePremiumAsync();

            decimal premium = await retailCascoPage.GetBasePremiumAsync();
            TestContext.WriteLine( $"Premium calculated: {premium}" );

            Assert.That(premium, Is.GreaterThan( 0m ), $"Premium calcualted: {premium}" );
        }

       

    }
}
