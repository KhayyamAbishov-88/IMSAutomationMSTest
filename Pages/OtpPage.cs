using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
using IMSAutomation.Pages;

    internal class OtpPage : BasePage
    {

        public OtpPage ( IPage page ) : base( page )
        {

        }

        public async Task<HomePage> ClickToLoginViaOtp ( string otpCode )
        {
            await page.Locator( "#OTPCode" ).FillAsync( otpCode );
            var verifyButton = page.Locator( "input[name='VerifyOTP']" );
            await verifyButton.ClickAsync();



            return new HomePage( page );
        }

    }

