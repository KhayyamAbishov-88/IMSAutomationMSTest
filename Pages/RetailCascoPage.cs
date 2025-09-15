using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework.Interfaces;
using static OfficeOpenXml.ExcelErrorValue;

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
            var validationMessage = page.Locator( "span[data-valmsg-for='Vehicle.RegNr']" );


            await input.ClickAsync();
            await input.FillAsync( regNum );
            await input.BlurAsync();


            if ( string.IsNullOrEmpty( regNum ) )
            {
                // Assert required validation message shown
                await Assertions.Expect( validationMessage ).ToContainTextAsync( "Xahiş olunur 'Qeydiyyat nömrəsi' qeyd edin" );
            }
            else if ( !System.Text.RegularExpressions.Regex.IsMatch( regNum, "^[a-zA-Z0-9]*$" ) )
            {
                // Assert regex validation message shown
                await Assertions.Expect( validationMessage ).ToContainTextAsync( "Yalnız rəqəm və hərf daxil edilə bilər" );
            }
            else
            {
                // If valid input, assert no validation message
                await Assertions.Expect( validationMessage ).ToBeEmptyAsync();
            }


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

        public async Task ClickSectionByNameAsync ( IPage page, string sectionTitle )
        {
            // Find the section-title <h2> by the inner <span> text
            var section = page.Locator( "h2.section-title" ).Filter( new() { HasTextRegex = new Regex( sectionTitle ) } );

            // Click the expand/collapse icon inside that section
            await section.Locator( ".sect-icon" ).ClickAsync();
        }

        public async Task SearchPolicyHolderInfo ( string pin, string series, string idNumber, string phone )
        {
            await page.Locator( "#Client_SearchParameters_PIN" ).ClickAsync();
            await page.Locator( "#Client_SearchParameters_PIN" ).PressSequentiallyAsync( pin );

            await page.Locator( "#Client_SearchParameters_IdSeries" ).ClickAsync();
            await page.Locator( "#Client_SearchParameters_IdSeries" ).PressSequentiallyAsync( series );

            await page.Locator( "#Client_SearchParameters_IdNumber" ).ClickAsync();
            await page.Locator( "#Client_SearchParameters_IdNumber" ).PressSequentiallyAsync( idNumber );

            await page.Locator( "#Client_SearchParameters_Search" ).ClickAsync();

            await page.Locator( "#Client_Phone" ).ClickAsync();
            await page.Locator( "#Client_Phone" ).PressSequentiallyAsync( phone );
        }

        public async Task ClickToCalculatePremiumAsync ()
        {
            await page.Locator( "#calculateButton" ).ClickAsync();
            await Assertions.Expect( page.Locator( "#calculateButton" ) ).Not.ToBeDisabledAsync();

            // Strategy 2: Wait for network idle if API calls are involved
            await page.WaitForLoadStateAsync( LoadState.NetworkIdle );
        }




        public async Task<decimal> GetBasePremiumAsync ()
        {
            var basePremiumInput = page.Locator( "#PolicyDiscount_BasePremium" );
            var culture = new System.Globalization.CultureInfo( "az-Latn-AZ" );

            // Wait until value is not "0,00" (max 5s)
            for ( int i = 0; i < 10; i++ )
            {
                var valueStr = await basePremiumInput.InputValueAsync();

                if ( valueStr != "0,00" )
                {
                    if ( decimal.TryParse( valueStr, System.Globalization.NumberStyles.Any, culture, out var parsedValue ) )
                        return parsedValue;

                    throw new FormatException( $"Could not parse base premium value: '{valueStr}'" );
                }

                await Task.Delay( 500 );
            }

            // Still "0,00" after waiting — return 0 to fail in test assertion
            return 0m;
        }


        public async Task<bool> ClicktoIssuePolicyAndCheckSuccessAsync ()
        {
            await page.Locator( "input[type='submit'][value='Polisi burax']" ).ClickAsync();


            // Locate the success message
            var successMessage = page.Locator( "p.success" );
            // Wait for the success message to appear
            await Assertions.Expect( page.Locator( "p.success" ) ).ToBeVisibleAsync();

            // Return whether it's visible
            return await successMessage.IsVisibleAsync();

        }




    }
}
   

