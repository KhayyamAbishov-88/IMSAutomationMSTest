using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework.Interfaces;

namespace IMSAutomation.Pages
{
    internal class RetailCascoPage : BasePage

    {

        public RetailCascoPage ( IPage page ) : base( page )
        {

        }

        private static readonly Random _random = new Random();

        public static string GetRandomVehicleRegNr ()
        {
            string digits = "0123456789";
            string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            // 2 digits
            string part1 = new string( Enumerable.Range( 0, 2 )
                .Select( _ => digits[_random.Next( digits.Length )] ).ToArray() );

            // 2 letters
            string part2 = new string( Enumerable.Range( 0, 2 )
                .Select( _ => letters[_random.Next( letters.Length )] ).ToArray() );

            // 3 digits
            string part3 = new string( Enumerable.Range( 0, 3 )
                .Select( _ => digits[_random.Next( digits.Length )] ).ToArray() );

            return part1 + part2 + part3;


        }


        public static string GetRandomVehicleCerNr ()
        {
            var random = new Random();
            // Generate two random uppercase letters
            char letter1 = ( char )random.Next( 'A', 'Z' + 1 );
            char letter2 = ( char )random.Next( 'A', 'Z' + 1 );

            // Generate a 6-digit random number
            int numberPart = random.Next( 100000, 1000000 ); // Ensures it's 6 digits

            return $"{letter1}{letter2}{numberPart}";
        }


        public async Task ClikcFillVechRegNumInputAsync ( string regNum )
        {
            var input = page.Locator( "#Vehicle_RegNr" );
            await input.ClickAsync();
            await input.FillAsync( regNum );
        }

        public async Task ClikcFillVechCertNumInputAsync ( string certNum )
        {
            var input = page.Locator( "#Vehicle_RegCertNumber" );
            await input.ClickAsync();
            await input.FillAsync( certNum );
        }



        public async Task SelectVehicleBrand ( string brand )
        {
            await page.WaitForSelectorAsync( "#Vehicle_VehBrandOidText" );
            await page.Locator( "#Vehicle_VehBrandOidText" ).ClickAsync();
            await page.Locator( "#Vehicle_VehBrandOidText" ).FillAsync( brand );
            //  await page.Locator( $"li:has-text('{optionText}')" ).ClickAsync();
        }


        public async Task SelectVehicleModel ( string model )
        {
            await page.WaitForSelectorAsync( "#Vehicle_VehModelOidText" );
            await page.Locator( "#Vehicle_VehModelOidText" ).ClickAsync();
            await page.Locator( "#Vehicle_VehModelOidText" ).FillAsync( model );
            //  await page.Locator( $"li:has-text('{optionText}')" ).ClickAsync();
        }


        public async Task SelectVehicleSubModel ( string submodel )
        {
            await page.WaitForSelectorAsync( "#Vehicle_VehSubModelNameText" );
            await page.Locator( "#Vehicle_VehSubModelNameText" ).ClickAsync();
            await page.Locator( "#Vehicle_VehSubModelNameText" ).FillAsync( submodel );
            //  await page.Locator( $"li:has-text('{optionText}')" ).ClickAsync();
        }


        public async Task ClikcFillVechManfactYearInputAsync ( string year )
        {
            var input = page.Locator( "#Vehicle_ManufactoryYear" );
            await input.ClickAsync();
            await input.FillAsync( year );
        }

        public async Task SelectVehicleUsage ()
        {
            // Type a character to trigger the dropdown
            await page.FillAsync( "#Vehicle_UsageOidText", "" );

            // Wait for at least one visible item in the dropdown
            await page.WaitForSelectorAsync( ".ui-menu-item >> visible=true" );

            // Get all visible dropdown items
            var items = await page.QuerySelectorAllAsync( ".ui-menu-item >> visible=true" );

            if ( items.Count > 0 )
            {
                var random = new Random();
                int index = random.Next( items.Count );

                // Click the randomly selected item
                await items[index].ClickAsync();
            }
            else
            {
                throw new Exception( "No visible items found in the dropdown." );
            }
        }

            public async Task SelectDeducible ()
            {
                // Type a character to trigger the dropdown
                await page.FillAsync( "#RetailCascoObject_DeductibleText", "" );

                // Wait for at least one visible item in the dropdown
                await page.WaitForSelectorAsync( ".ui-menu-item >> visible=true" );

                // Get all visible dropdown items
                var items = await page.QuerySelectorAllAsync( ".ui-menu-item >> visible=true" );

                if ( items.Count > 0 )
                {
                    var random = new Random();
                    int index = random.Next( items.Count );

                    // Click the randomly selected item
                    await items[index].ClickAsync();
                }
                else
                {
                    throw new Exception( "No visible items found in the dropdown." );
                }



            }
    }
}
