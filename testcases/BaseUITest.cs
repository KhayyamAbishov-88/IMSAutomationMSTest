using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Extensions.Configuration;
using IMSAutomation.utilities;
using IMSAutomation.Pages;
using Microsoft.EntityFrameworkCore;

namespace IMSAutomation.testcases
{
    public class BaseUITest

    {
        protected IPlaywright playwright;
      
        IConfiguration configuration;
       

        [OneTimeSetUp]
        public async Task BeforeAllTests()
        {

        }

        [SetUp]
        public async Task BeforeEachtest()
        {
           
            playwright = await Playwright.CreateAsync();
            LoginPage loginPage = new LoginPage(null);


        }

        protected async Task<(IBrowser, IPage)> CreateBrowserAndPage ( IPlaywright playwright, string browserType, BrowserTypeLaunchOptions launchOptions )
        {
            IBrowser browser;

            if ( browserType.Equals( "chrome", StringComparison.OrdinalIgnoreCase ) )
            {
                browser = await playwright.Chromium.LaunchAsync( launchOptions );
            }
            else if ( browserType.Equals( "firefox", StringComparison.OrdinalIgnoreCase ) )
            {
                browser = await playwright.Firefox.LaunchAsync( launchOptions );
            }
            else if ( browserType.Equals( "webkit", StringComparison.OrdinalIgnoreCase ) )
            {
                browser = await playwright.Webkit.LaunchAsync( launchOptions );
            }
            else
            {
                Assert.Fail( $"Unsupported browser type: {browserType}" );
                return (null, null);
            }

            // ✅ Create context with SSL errors ignored
            var context = await browser.NewContextAsync( new BrowserNewContextOptions
            {
                IgnoreHTTPSErrors = true
            } );

            var page = await context.NewPageAsync();
            await page.SetViewportSizeAsync( 1280, 720 );

            string url = "https://testserver01-polis.ateshgah.com/WebIMS/Account/Login";
            await page.GotoAsync( url );

            return (browser, page);
        }


        [TearDown]
        public async Task AfterEachTest ()
        {
           await Task.Delay( 5000 ); // Optional delay to observe the state after each test
        }

        [OneTimeTearDown]
        public async Task AfterAllTests ()
        {
            if ( playwright != null )
            {
                playwright.Dispose();
            }
        }
    }
}
