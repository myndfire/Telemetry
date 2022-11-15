using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Diagnostics.Metrics;
using System.Diagnostics;
using System.Net.Http;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry;
using OpenTelemetry.Logs;

namespace Otel.ServiceA
{
    public class Startup
    {
        // Create a route (GET /) that will make an http call, increment a metric and log a trace
        public static ActivitySource activitySource = new ActivitySource("WeatherForecastController");
        public static Meter meter = new Meter("WeatherForecastControllerMetrics");
        public static Counter<int> requestCounter = meter.CreateCounter<int>("compute_requests");

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            

            //----- OTEL ---
            // This is required if the collector doesn't expose an https endpoint as .NET by default
            // only allow http2 (required for gRPC) to secure endpoints
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //--- Otel-Metrics
            services.AddOpenTelemetryMetrics(builder =>
            {
                builder.AddHttpClientInstrumentation();
                builder.AddAspNetCoreInstrumentation();
                builder.AddMeter("MyApplicationMetrics");
                builder.AddOtlpExporter(options => options.Endpoint = new Uri("http://localhost:4317"));
            });

            //--- Otel-Tracing
            services.AddOpenTelemetryTracing(builder =>
            {
                builder.AddHttpClientInstrumentation();
                builder.AddAspNetCoreInstrumentation();
                builder.AddSource("MyApplicationActivitySource");
                builder.AddOtlpExporter(options => options.Endpoint = new Uri("http://localhost:4317"));
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Otel.ServiceA", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Otel.ServiceA v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
