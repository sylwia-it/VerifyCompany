using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace VerifyWhiteListCompany.Lib.Test
{
    
    static class UrlProvider
    {
        private static string _prodEnvUrl = null;
        private static string _testEnvUrl = null;
        public static string ProductionUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_prodEnvUrl))
                {
                    GetUrls();
                }
                return _prodEnvUrl;
            }
        }
        
        public static string TestUrl { get { if (string.IsNullOrEmpty(_testEnvUrl)){ GetUrls(); } return _testEnvUrl; } }

        private static void GetUrls()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();
            _prodEnvUrl = root.GetSection("URLs").GetSection("ProdEnv").Value;
            _testEnvUrl = root.GetSection("URLs").GetSection("TestEnv").Value;
        }
    }
}
