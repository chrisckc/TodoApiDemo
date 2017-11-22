using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;

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
                .Build();
    }
}
