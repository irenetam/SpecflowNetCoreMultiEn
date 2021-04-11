using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altsource
{
    public static class Helper
    {
        public static MyConfig GetApplicationConfiguration()
        {

            var currentDirectory = System.IO.Directory.GetCurrentDirectory();

            var iConfig = GetIConfigurationRoot(currentDirectory);

            //Microsoft.Extensions.Configuration.Binder.dll
            MyConfig configuration = iConfig.Get<MyConfig>();

            return configuration;
        }

        public static IConfiguration GetIConfigurationRoot(string outputPath)
        {
            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.json")
                .Build();
        }
    }
}
