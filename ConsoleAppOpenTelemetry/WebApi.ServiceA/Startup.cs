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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace WebApi.ServiceA
{
    public class Startup
    {
        public static ActivitySource _ActivitySource = new ActivitySource("WebApi.ServiceA", "1.0.0");

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient("WebApi.ServiceB")
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5001"));

            services.AddControllers();
            services.AddOpenTelemetryTracing(
                (builder) => builder
                    .AddAspNetCoreInstrumentation() // Picks trace from incoming header requests
                    .AddHttpClientInstrumentation() // Adds trace to header for outgoing requests
                    .AddSource("WebApi.ServiceA")
                                                    //.AddSqlClientInstrumentation(options =>
                                                    //{
                                                    //    options.SetDbStatementForText = true;
                                                    //    options.RecordException = true;
                                                    //})
                                                    //.AddConsoleExporter()
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService("WebApi.ServiceA"))
                    .AddJaegerExporter(configure: config =>
                    {
                        config.AgentHost = "localhost";
                        config.AgentPort = 6831;
                    })
            );

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi.ServiceA", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi.ServiceA v1"));
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
