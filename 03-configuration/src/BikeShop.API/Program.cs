using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// next 2 lines added in lab #2
using Steeltoe.Extensions.Configuration;
//using Steeltoe.Extensions.Configuration.CloudFoundry;

// above line commented out, next line added in lab #3
using Steeltoe.Extensions.Configuration.ConfigServer;


namespace BikeShop.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("ENV:"+Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                // next 27 lines added in lab #2
                .UseKestrel()
                .UseCloudFoundryHosting()
                .UseContentRoot(Directory.GetCurrentDirectory())
                // .UseIISIntegration() - s/b unnecessary unless using IIS
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((builderContext, configBuilder) =>
                {
                    // var csc = new ConfigServerClientSettings()
                    // {
                    //     AccessTokenUri ="https://p-spring-cloud-services.uaa.run.pivotal.io/oauth/token",
                    //     ClientId = "7JVIcWfmGZhj",
                    //     ClientSecret = "p-config-server-38a24fce-0a5d-4eab-b6f2-0ee9ea5fb8dc",
                    //     Uri = "https://config-f0ca266d-8453-4471-9749-5fc542bfc315.cfapps.io"
                    // };
                    var env = builderContext.HostingEnvironment;
                    configBuilder.SetBasePath(env.ContentRootPath)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                        .AddEnvironmentVariables()
                        // .AddCloudFoundry()
                        .AddConfigServer(env);
                })
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddConfiguration(context.Configuration.GetSection("Logging"));
                });
    }
}
