using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace IMSAutomation.pages

{
    internal class HomePage
    {
        IPage page;
        public async void Products()
        {
            await page.GetByRole( AriaRole.Link, new() { Name = "Məhsullar" } ).HoverAsync();
            await page.GetByRole( AriaRole.Link, new() { Name = "Agent Kaskosu" } ).ClickAsync();


        }
    }
}
