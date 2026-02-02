using log4net;
using log4net.Config;
using System.IO;
using System.Reflection;

namespace IMSAutomation.utilities
{
    public static class LoggerHelper
    {
        private static bool _isConfigured = false;
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static ILog GetLogger(System.Type type)
        {
            if (!_isConfigured)
            {
                var repository = LogManager.GetRepository(Assembly.GetEntryAssembly());
                var configFile = new FileInfo("log4net.config");
                XmlConfigurator.Configure(repository, configFile);
                _isConfigured = true;
            }
            return LogManager.GetLogger(type);
        }
    }
}
