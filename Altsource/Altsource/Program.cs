
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Altsource
{
    public class Program
    {
        //public static IConfigurationRoot configuration;

        //public static void Main(string[] args)
        //{
        //    var builder = new ConfigurationBuilder()
        //       .SetBasePath(Directory.GetCurrentDirectory())
        //       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        //       .AddEnvironmentVariables();

        //    IConfigurationRoot configuration = builder.Build();

        //    var mySettingsConfig = new MyConfig();
        //    configuration.GetSection("MyConfig").Bind(mySettingsConfig);

        //    Console.WriteLine("Setting from appsettings.json: " + mySettingsConfig.ApplicationName);
        //    Console.WriteLine("Setting from secrets.json: " + mySettingsConfig.Version);
        //}
        //public static void Main(string[] args)
        //{
        //    BuildWebHost(args).Run();
        //}

        //public static IWebHost BuildWebHost(string[] args)
        //{
        //    return WebHost.CreateDefaultBuilder(args)
        //        .UseStartup<Startup>()
        //        .Build();
        //}

        public static void Main(string[] args)
        {
            var appSettings = JObject.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json")));
            var environmentValue = appSettings["Environment"].ToString();

            var webHostBuilder = CreateWebHostBuilder(args);

            if (!String.IsNullOrEmpty(environmentValue))
            {
                webHostBuilder.UseEnvironment(environmentValue);
            }

            var host = webHostBuilder.Build();
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
