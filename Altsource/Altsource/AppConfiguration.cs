using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Altsource
{
    public class AppConfiguration
    {
        // We are testing this >>>
        public static IConfigurationRoot configuration;

        private IConfigurationRoot GetIConfigurationRoot()
        {
            try
            {
                // Build configuration

                var appSettings = JObject.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json")));
                var environmentName = appSettings["Environment"].ToString();
                configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environmentName}.json", true)
                    .AddEnvironmentVariables()
                    .Build();
            }
            catch (Exception e)
            {
                // Error loading the application configuration file 
            }

            return configuration;
        }


        public AppConfiguration GetApplicationConfiguration()
        {
            var configuration = new AppConfiguration();

            var iConfig = GetIConfigurationRoot();

            iConfig
                .GetSection("MyConfig")
                .Bind(configuration);

            //var appSettings = JObject.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json")));
            //var environmentValue = appSettings["Environment"].ToString();

            //if (!String.IsNullOrEmpty(environmentValue))
            //{
            //    webHostBuilder.UseEnvironment(environmentValue);
            //}

            //var host = webHostBuilder.Build();

            return configuration;
        }
    }
}
