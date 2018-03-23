using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using EventWebApp.Models;
using EventWebApp.Service;
using EventWebApp.Models.Filter;
using Microsoft.EntityFrameworkCore;

namespace EventWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient< IDaoService < Event, EventRequestFilter >, EventDaoService>();
            services.AddMvc();
            services.AddApiVersioning();
            var connection = @"Server=(localdb)\mssqllocaldb;Database=EventDatabase;Trusted_Connection=True;ConnectRetryCount=0";
            services.AddDbContext<Models.EventDatabaseContext>(options => options.UseSqlServer(connection));
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
