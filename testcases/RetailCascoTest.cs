using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Azure;

using IMSAutomation.Pages;
using Microsoft.Playwright;
using IMSAutomation.utilities;
    


    namespace IMSAutomation.TestCases
    {
        [TestFixture]
        [Parallelizable]
        public class RetailCascoTest : BaseUITest
        {
            private const string ConnectionString = "Server=testserver01;Database=Eagle;User Id=sa_eagle;Password=Pony3201;TrustServerCertificate=True;";
            private const string UserLogin = "1-1-2-15";
            private const string UserPassword = "Aa123456789";
            private DateTime DateTime ;
            private async Task<RetailCascoPage> PrepareRetailCascoPage ( IPage page )
            {
                LoginPage loginPage = new LoginPage( page );
                BasePage resultPage = await loginPage.RedirectPageAfterLogin( UserLogin, UserPassword );
            if ( resultPage is HomePage homePage )
            {
                await homePage.ClickProducts( new ProductsPage( page ) );
            }
            else
            {
                Assert.Fail( "Expected HomePage after successful login." );
            }

            var retailCascoPage = new RetailCascoPage( page );
                await retailCascoPage.ClikcFillVechRegNumInputAsync( RetailCascoPage.GetRandomVehicleRegNr() );
                await retailCascoPage.ClikcFillVechCertNumInputAsync( RetailCascoPage.GetRandomVehicleCerNr() );

                var vehicle = new DatabaseHelper().GetRandomVehicleFromView( ConnectionString );

                await retailCascoPage.SelectVehicleBrand( vehicle.BrandName );
                await retailCascoPage.SelectVehicleModel( vehicle.ModelName );
                await retailCascoPage.SelectVehicleSubModel( vehicle.SubModel );
                await retailCascoPage.ClikcFillVechManfactYearInputAsync( ( DateTime.Now.Year - vehicle.YearOld ).ToString() );
                await retailCascoPage.SelectVehicleUsage();
                await retailCascoPage.SelectDeducible();

                await retailCascoPage.ClickSectionByNameAsync( page, "Sığorta" );
                await retailCascoPage.SearchPolicyHolderInfo( "4Z6NNQZ", "AA", "6095063", "994512068475" );

                return retailCascoPage;
            }

            [Test]
            public async Task RetailCasco_CalculatePremium_ShouldBeGreaterThanZero ()
            {
                var (_, page) = await CreateBrowserAndPage( playwright, "chrome", new BrowserTypeLaunchOptions { Headless = false } );

                var cascoPage = await PrepareRetailCascoPage( page );
                await cascoPage.ClickToCalculatePremiumAsync();

                decimal premium = await cascoPage.GetBasePremiumAsync();
                Assert.That( premium, Is.GreaterThan( 0m ), $"Premium should be greater than zero but was: {premium}" );
            }

            [Test]
            public async Task RetailCasco_PhysicalPersonPolicyIssue_ShouldSucceed ()
            {
                var (_, page) = await CreateBrowserAndPage( playwright, "chrome", new BrowserTypeLaunchOptions { Headless = false } );

                var cascoPage = await PrepareRetailCascoPage( page );
                await cascoPage.ClickToCalculatePremiumAsync();
             

                 bool isSuccess = await cascoPage.ClicktoIssuePolicyAndCheckSuccessAsync();

            Assert.That( isSuccess, Is.True, "Success message should appear after issuing the policy." );
            }
        }
    }




