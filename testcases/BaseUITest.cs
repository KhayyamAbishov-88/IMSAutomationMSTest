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

namespace IMSAutomation.TestCases
{
    public class BaseUITest

    {
        protected IPlaywright playwright;
        protected IBrowser browser;
        protected IPage page; // Made protected to be accessible for screenshots
        IConfiguration configuration;
        private static readonly log4net.ILog log = LoggerHelper.GetLogger(typeof(BaseUITest));

        [OneTimeSetUp]
        public async Task BeforeAllTests()
        {
            log.Info("Starting Test Run");
        }

        [SetUp]
        public async Task BeforeEachtest()
        {
            log.Info($"Starting test: {TestContext.CurrentContext.Test.Name}");
            playwright = await Playwright.CreateAsync();
            LoginPage loginPage = new LoginPage(null);
        }

        protected async Task<(IBrowser, IPage)> CreateBrowserAndPage(IPlaywright playwright, string browserType, BrowserTypeLaunchOptions launchOptions)
        {
            IBrowser browser;

            if (browserType.Equals("chrome", StringComparison.OrdinalIgnoreCase))
            {
                browser = await playwright.Chromium.LaunchAsync(launchOptions);
            }
            else if (browserType.Equals("firefox", StringComparison.OrdinalIgnoreCase))
            {
                browser = await playwright.Firefox.LaunchAsync(launchOptions);
            }
            else if (browserType.Equals("webkit", StringComparison.OrdinalIgnoreCase))
            {
                browser = await playwright.Webkit.LaunchAsync(launchOptions);
            }
            else
            {
                var msg = $"Unsupported browser type: {browserType}";
                log.Error(msg);
                Assert.Fail(msg);
                return (null, null);
            }

            // ✅ Create context with SSL errors ignored
            var context = await browser.NewContextAsync(new BrowserNewContextOptions
            {
                IgnoreHTTPSErrors = true,
                AcceptDownloads = false,
                HasTouch = false,
                // Don't use any existing storage
                StorageState = null
            });

            await context.ClearCookiesAsync();

            this.page = await context.NewPageAsync();

            await page.SetViewportSizeAsync(1280, 720);

            string url = "https://testserver01-polis.ateshgah.com/WebIMS/Account/Login";
            log.Info($"Navigating to URL: {url}");
            await page.GotoAsync(url);

            return (browser, page);
        }


        [TearDown]
        public async Task AfterEachTest()
        {
            var outcome = TestContext.CurrentContext.Result.Outcome.Status;
            if (outcome == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                var message = TestContext.CurrentContext.Result.Message;
                var stackTrace = TestContext.CurrentContext.Result.StackTrace;
                log.Error($"Test Failed: {TestContext.CurrentContext.Test.Name}");
                log.Error($"Message: {message}");
                log.Error($"StackTrace: {stackTrace}");

                if (page != null)
                {
                    await ScreenshotHelper.CaptureScreenshotAsync(page, TestContext.CurrentContext.Test.Name);
                }
            }
            else
            {
                log.Info($"Test Passed: {TestContext.CurrentContext.Test.Name}");
            }

            await Task.Delay(1000); // Optional delay to observe the state after each test
        }

        [OneTimeTearDown]
        public async Task AfterAllTests()
        {
            if (playwright != null)
            {
                playwright.Dispose();
            }
            log.Info("Test Run Finished");
        }


        protected async Task CloseBrowserAsync()
        {
            if (browser != null)
            {
                await browser.CloseAsync();
                browser = null;
            }
        }
    }
}
