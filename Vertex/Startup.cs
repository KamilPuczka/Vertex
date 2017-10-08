using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Api.Authorization;
using Vertex.Helpers;
using Vertex.Services;
using Services;

namespace Vertex
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton(new ConfigGetterHelper(Configuration));
            services.AddScoped<IGetOperations, GetOperations>();
            services.AddScoped<IEncrypter, Encrypter>();
            // Add framework services.
            services.AddAuthentication();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseOAuthValidation();
            app.UseOpenIdConnectServer(options =>
            {
                options.Provider = new AuthorizationProvider();
                options.TokenEndpointPath = "/connect/token";
                options.AccessTokenLifetime = options.IdentityTokenLifetime = TimeSpan.FromHours(1);
                options.AllowInsecureHttp = true;
            });


            app.UseMvc();
        }
    }
}
