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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IMSAutomation.TestCases
{
    [TestClass]
    public class BaseUITest
    {
        protected IPlaywright playwright;
        protected IBrowser browser;
        protected IPage page; // Made protected to be accessible for screenshots
        protected IConfiguration configuration;
        public TestContext TestContext { get; set; }
        private static readonly log4net.ILog log = LoggerHelper.GetLogger(typeof(BaseUITest));

        [ClassInitialize]
        public static void BeforeAllTests(TestContext context)
        {
            log4net.ILog classLog = LoggerHelper.GetLogger(typeof(BaseUITest));
            classLog.Info("Starting Test Run");
        }

        [TestInitialize]
        public async Task BeforeEachtest()
        {
            log.Info($"Starting test: {TestContext.TestName}");
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


        [TestCleanup]
        public async Task AfterEachTest()
        {
            var outcome = TestContext.CurrentTestOutcome;
            if (outcome == UnitTestOutcome.Failed)
            {
                log.Error($"Test Failed: {TestContext.TestName}");

                if (page != null)
                {
                    await ScreenshotHelper.CaptureScreenshotAsync(page, TestContext.TestName);
                }
            }
            else
            {
                log.Info($"Test Passed: {TestContext.TestName}");
            }

            await Task.Delay(1000); // Optional delay to observe the state after each test
        }

        [ClassCleanup]
        public static void AfterAllTests()
        {
            // Note: playwright and browser are instance members, so they can't be easily disposed here
            // If they need to be disposed at class level, they should be static or handled in TestCleanup
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
