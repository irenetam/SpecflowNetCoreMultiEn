using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Altsource
{
    public class Startup
    {
        //public IConfigurationRoot Configuration { get; set; }
        //public Startup()
        //{
        //    var builder = new ConfigurationBuilder()
        //       .SetBasePath(Directory.GetCurrentDirectory())
        //       .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        //       .AddEnvironmentVariables();

        //    Configuration = builder.Build();
        //}
        //public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        //{
        //    if (env.IsDevelopment())
        //    {
        //        app.UseDeveloperExceptionPage();
        //    }
        //}
        //public void ConfigureServices(IServiceCollection services)
        //{
        //    // Add functionality to inject IOptions<T>
        //    services.AddOptions();

        //    // Add our Config object so it can be injected
        //    //services.Configure<MyConfig>(Configuration.GetSection("MyConfig"));
        //    services.Configure<MyConfig>(option =>
        //    {
        //        option.ApplicationName = Configuration.GetSection("ApplicationName").Value;
        //        option.Version = Configuration.GetSection("Version").Value;
        //        Console.WriteLine(option.ApplicationName);
        //    });
        //}
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();


            Configuration = builder.Build();
        }
        public void ConfigureServices(IServiceCollection services)
        {
             services.AddOptions();

            // Add our Config object so it can be injected
            //services.Configure<MyConfig>(Configuration.GetSection("MyConfig"));
            services.Configure<MyConfig>(option =>
            {
                option.ApplicationName = Configuration.GetSection("ApplicationName").Value;
                option.Version = Configuration.GetSection("Version").Value;
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        }
    }
}
