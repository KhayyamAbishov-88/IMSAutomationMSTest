using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace IMSAutomation.Pages
{
    internal class RetailCascoPage :BasePage

    {
        
        public RetailCascoPage(IPage page) : base(page)
        {
            
        }

        private static readonly Random _random = new Random();

        public static string GenerateCustomCode ()
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



    }
}
