using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// next 5 lines added in lab #2
using BikeShop.API.Services;
using BikeShop.API.Repositories;
using BikeShop.API.Data;
using Steeltoe.CloudFoundry.Connector.MySql;
using Steeltoe.CloudFoundry.Connector.MySql.EFCore;

//next 2 lines added in lab #3
using Steeltoe.Extensions.Configuration.ConfigServer;
using BikeShop.API.Models;

//<debug only>
using Steeltoe.CloudFoundry.Connector;
using Steeltoe.CloudFoundry.Connector.Services;

namespace BikeShop.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            // Not requireed, but may be helpful for deubgging connection issues
            //var info = configuration.GetRequiredServiceInfo<MySqlServiceInfo>("mysql");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // next 4 lines added in lab #2
            services.AddMySqlConnection(Configuration);
            services.AddTransient<BicycleService>();
            services.AddTransient<BicycleRepository>();
            services.AddDbContext<BicycleDbContext>(o => o.UseMySql(Configuration, "mysql"));

            //next 2 lines add in lab #3
            services.AddOptions();
            // services.ConfigureConfigServerClientOptions(Configuration);

            // var test = Configuration.GetSection("headerMessage");
            // if(test.Value == null)
            // {
            //     Console.WriteLine("No 'headerMessage' section in configuration!");
            // }
            services.Configure<HeaderMessageConfiguration>(Configuration.GetSection("headerMessage"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider sp)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //next line added in lab #3
            app.UseStaticFiles();

            app.UseHttpsRedirection();
            app.UseMvc();

            // next line added in lab #2
            //BicycleDbInitialize.init(app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider);

            using(var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                BicycleDbInitialize.init(scope.ServiceProvider);
            }
        }
    }
}
