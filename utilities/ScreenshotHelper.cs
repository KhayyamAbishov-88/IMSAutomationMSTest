using Microsoft.Playwright;
using System;
using System.IO;
using System.Threading.Tasks;

namespace IMSAutomation.utilities
{
    public static class ScreenshotHelper
    {
        public static async Task CaptureScreenshotAsync(IPage page, string testName)
        {
            if (page == null) return;

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"{testName}_{timestamp}.png";
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string filePath = Path.Combine(folderPath, fileName);
            await page.ScreenshotAsync(new PageScreenshotOptions { Path = filePath, FullPage = true });
            
            Console.WriteLine($"Screenshot saved: {filePath}");
        }
    }
}
