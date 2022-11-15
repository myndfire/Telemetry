

using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace WebApi.ServiceB
{
    public class Startup
    {
        public static ActivitySource _ActivitySource = new ActivitySource("WebApi.ServiceB", "1.0.0");

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddOpenTelemetryTracing(
                (builder) => builder
                    .AddAspNetCoreInstrumentation() // Picks trace from incoming header requests
                    .AddHttpClientInstrumentation() // Adds trace to header for outgoing requests
                    .AddSource("WebApi.ServiceB")
                    //.AddSqlClientInstrumentation(options =>
                    //{
                    //    options.SetDbStatementForText = true;
                    //    options.RecordException = true;
                    //})
                    //.AddConsoleExporter()
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("WebApi.ServiceB"))
                    .AddJaegerExporter(configure: config =>
                    {
                        config.AgentHost = "localhost";
                        config.AgentPort = 6831;
                    })
            );
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi.ServiceB", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi.ServiceB v1"));
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
