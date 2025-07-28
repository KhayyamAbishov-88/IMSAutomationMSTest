using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;

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


        public async Task ClikcFillInputAsync ( string selector, string value )
        {
            var input = page.Locator( selector );
            await input.ClickAsync();
            await input.FillAsync( value );
        }


        public async Task SelectDropdownItem ( string inputSelector, string optionText )
        {
            await page.Locator( inputSelector ).ClickAsync();
            await page.Locator( inputSelector ).FillAsync( optionText );
            await page.Locator( $"li:has-text('{optionText}')" ).ClickAsync();
        }



    }
}
