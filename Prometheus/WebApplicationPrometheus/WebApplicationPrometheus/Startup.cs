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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace WebApplicationPrometheus
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
            services.AddControllers();
            services.AddSingleton<WeatherForecastMetricsCollector>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();

            app.Map("/metrics", metricsApp =>
            {
                //metricsApp.UseMiddleware<BasicAuthMiddleware>("mycorp");

                // We already specified URL prefix in .Map() above, no need to specify it again here.
                metricsApp.UseMetricServer("");
            });

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapMetrics();
                endpoints.MapControllers();
            });
            Metrics.SuppressDefaultMetrics();
        }
    }
}
