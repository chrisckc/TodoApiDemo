using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using TodoApi.Filters;
using TodoApi.Models;
using TodoApi.Data;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using TodoApiDemo.Handlers;
using System;

namespace TodoApi
{
    public class Startup
    {

        //public IConfigurationRoot Configuration { get; }  // previous dotnet 1.0 method
        public IConfiguration Configuration { get; }
        public ILoggerFactory LoggerFactory { get; }

        // previous dotnet 1.0 method
        // public Startup(IHostingEnvironment env, ILoggerFactory logger)
        // {
        //     this.LoggerFactory = logger;

        //     var builder = new ConfigurationBuilder()
        //         .SetBasePath(env.ContentRootPath)
        //         .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        //         .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
        //         .AddEnvironmentVariables();
        //     Configuration = builder.Build();
        // }

        public Startup(IConfiguration configuration, ILoggerFactory logger)
        {
            this.Configuration = configuration;
            this.LoggerFactory = logger;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        #region snippet_AddSingleton
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            var builder = services.AddMvc();
            // Setup global exception filter
            //var builder = services.AddMvc(config => config.Filters.Add(typeof(GlobalExceptionFilter)));

            builder.AddJsonOptions(
                o =>
                {
                    o.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    o.SerializerSettings.Converters.Add(new StringEnumConverter());
                    o.SerializerSettings.Formatting = Formatting.Indented;
                    o.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                    o.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                    //o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            services.AddResponseCompression();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IExceptionHandler, ExceptionHandler>();
            services.AddScoped<ITodoRepository, TodoRepository>();
        }
        #endregion

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider, IHostingEnvironment env)
        {
            IExceptionHandler exceptionHandler = serviceProvider.GetService<IExceptionHandler>();
            //env.EnvironmentName = EnvironmentName.Development;
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                exceptionHandler.IncludeException = true;
                exceptionHandler.IncludeExceptionStacktrace = true;
            } else {
                //app.UseExceptionHandler("/api/error");
            }

            app.UseExceptionHandler(options => 
                options.Run(async context => await exceptionHandler.HandleException(context)));
            
            // serves static files from wwwroot dir
            app.UseStaticFiles();
            
            // This must be enabled this to catch errors that occur during serialization
            //app.UseResponseBuffering();

            // app.UseResponseCompression();
            app.UseMvc();
        }
    }
}
