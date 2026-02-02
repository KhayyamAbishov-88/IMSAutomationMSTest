using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSAutomation.Exceptions
{
    internal class LoginException : AutomationException
    {
        public LoginException(string message) : base(message)
        {
        }
        public LoginException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
