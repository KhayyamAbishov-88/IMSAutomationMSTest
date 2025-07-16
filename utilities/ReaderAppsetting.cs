using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSAutomation.utilities
{
    using System;
    using System.IO;
    using Microsoft.Extensions.Configuration;

    internal class ReaderAppsetting
    {
        private static IConfiguration _configuration;

        static ReaderAppsetting()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + "\\resources\\")
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        public static string GetSetting(string key)
        {
            return _configuration [ key ];
        }
    }

}
