using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;

namespace TodoApi
{
    public class Program
    {
        // previous dotnet 1.0 method
        // public static void Main(string[] args)
        // {
        //     var host = new WebHostBuilder()
        //         .UseKestrel()
        //         .UseContentRoot(Directory.GetCurrentDirectory())
        //         .UseUrls("http://localhost:5000", "http://192.168.0.2:5000")
        //         .UseIISIntegration()
        //         .UseStartup<Startup>()
        //         .Build();

        //     host.Run();
        // }

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                // optional config to remove the default configuration providers and add custom ones
                // .ConfigureAppConfiguration((hostContext, config) =>
                // {
                //     // delete all default configuration providers
                //     config.Sources.Clear();
                //     config.AddJsonFile("myconfig.json", optional: true);
                // })
                .UseKestrel(options =>
                {
                    options.Limits.MaxConcurrentConnections = 100;
                    options.Limits.MaxConcurrentUpgradedConnections = 100;
                    options.Limits.MaxRequestBodySize = 10 * 1024;
                    // The default minimum rates are 240 bytes / second, with a 5 second grace period.
                    options.Limits.MinRequestBodyDataRate =
                               new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                    options.Limits.MinResponseDataRate =
                               new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                    //options.Listen(IPAddress.Loopback, 5000);
                    options.Listen(IPAddress.Any, 5000);
                   //options.Listen(IPAddress.Loopback, 5001, listenOptions =>
                   //{
                   //    listenOptions.UseHttps("testCert.pfx", "testPassword");
                   //});
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    // these are added by the default config, AddConsole(); AddDebug();
                    logging.ClearProviders();
                    if (hostingContext.HostingEnvironment.IsDevelopment())
                    {
                        logging.AddConsole();
                        logging.AddDebug();
                        // this can be controlled in the appsettings.json
                        //logging.SetMinimumLevel(LogLevel.Debug);
                        // or
                        //logging.WithFilter<ConsoleLoggerProvider>(LogLevel.Debug))
                    }
                })
                .Build();
    }
}
