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

namespace TodoApi
{
    public class Startup
    {

        public IConfigurationRoot Configuration { get; }
        public ILoggerFactory LoggerFactory { get; }

        public Startup(IHostingEnvironment env, ILoggerFactory logger)
        {
            this.LoggerFactory = logger;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        #region snippet_AddSingleton
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            var builder = services.AddMvc();

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

            // Setup global exception filter
            builder.AddMvcOptions(o => { o.Filters.Add(new GlobalExceptionFilter(this.LoggerFactory)); });

            services.AddSingleton<ITodoRepository, TodoRepository>();
        }
        #endregion

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
