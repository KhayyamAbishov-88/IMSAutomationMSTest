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

namespace IMSAutomation.testcases
{
    internal class BaseUITest

    {
       protected  IPlaywright playwright;
        IConfiguration configuration;


        [SetUp]
        public async Task BeforeEachtest()
        {
            playwright = await Playwright.CreateAsync();
            var (Browser, page) = await CreateBrowserAndPage( playwright, "chrome", new BrowserTypeLaunchOptions { Headless = null } );
            LoginPage loginPage = new LoginPage(page);
            loginPage.LoginCredentials( "1-1-2-15", "Aa123456789" );
        }

        protected async Task<(IBrowser, IPage)> CreateBrowserAndPage ( IPlaywright plawrgt, string browserType, BrowserTypeLaunchOptions launchOptions = null )
        {
            IBrowser browser;
            if ( browserType.Equals( "chrome", StringComparison.OrdinalIgnoreCase ) )
            {
                browser = await plawrgt.Chromium.LaunchAsync( launchOptions );
            }
            else if ( browserType.Equals( "firefox", StringComparison.OrdinalIgnoreCase ) )
            {
                browser = await plawrgt.Firefox.LaunchAsync( launchOptions );
            }
            else if ( browserType.Equals( "webkit", StringComparison.OrdinalIgnoreCase ) )
            {
                browser = await plawrgt.Webkit.LaunchAsync( launchOptions );
            }
            else
            {
                Assert.Fail( $"Unsupported browser type: {browserType}" );
                return (null, null);

            }

            IPage page = await browser.NewPageAsync();
            await page.SetViewportSizeAsync( 1280, 720 );

            string url = ReaderAppsetting.GetSetting( configuration["AppSettings:testsiteurl"] );
            await page.GotoAsync( url );

            return (browser, page);

        }

        [TearDown]
        public async Task AfterEachTest()
        {
             playwright.Dispose();
        }

    }
}
