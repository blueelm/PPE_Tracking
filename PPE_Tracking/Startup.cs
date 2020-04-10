using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using PPE_Tracking.Data;
using PPE_Worker_Service;
using System.Text.Json.Serialization;

namespace PPE_Tracking
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddDbContext<PPE_Models.SQLiteDBContext>();
                
            // Enable below to use DR as data source
            services.AddScoped<IDataSource, DataRepository>(sp => new DataRepository(Configuration["ConnectionString"]));
            services.AddHostedService<Worker>(sp => new Worker(sp.GetService<ILogger<Worker>>(), Configuration["ConnectionString"], int.Parse(Configuration["RefreshInterval"])));
            
            // Enable below to use OpenGate SQL CLR integration instead of DR.
            // services.AddScoped<IDataSource, OpenGate>(sp => new OpenGate(Configuration["ConnectionString"],Configuration["OpenGateConnectionString"]));
            // services.AddHostedService<Worker>(sp => new OpenGateWorker(sp.GetService<ILogger<OpenGateWorker>>(), Configuration["ConnectionString"],  int.Parse(Configuration["RefreshInterval"]), Configuration["OpenGateConnectionString"]));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
          
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
