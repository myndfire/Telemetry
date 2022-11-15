using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ClassLibraryA;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Platform.Logging.Api;
using Platform.Logging.Api.Filters;
using Platform.Logging.Api.Middleware;
using Serilog;
using Serilog.Events;

namespace WebApiSerilogElasticsearch
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
            services.AddTransient<RequestManager>();
            services.AddTransient<RequestHandler>();

            //Add Performance Filter
            //services.AddSingleton<IScopeInformation, ScopeInformation>();
            //services.AddScoped<TrackActionPerformanceFilter> ();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Custom Exception Logging
            app.UseCustomApiExceptionHandler(options =>
            {
                options.AddResponseDetails = CustomErrorFormatter;
                options.DetermineLogLevel = DetermineLogLevel;
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


        // These 2 methods are application dependent and need to be customized as such
        private LogLevel DetermineLogLevel(Exception ex)
        {
            if (ex.Message.StartsWith("cannot open database", StringComparison.InvariantCultureIgnoreCase) ||
                ex.Message.StartsWith("a network-related", StringComparison.InvariantCultureIgnoreCase))
            {
                return LogLevel.Critical;
            }
            return LogLevel.Error;
        }
        private void CustomErrorFormatter(HttpContext context, Exception ex, ApiError error)
        {
            if (ex.GetType().Name == nameof(SqlException))
            {
                error.Detail = "Exception was a database exception!";
                //error.Links = "https://gethelpformyerror.com/";
            }
            else
            {
                error.Title= "Oops..Something went wrong..Sorry";
                error.Detail = "Top Secret";
            }

        }
    }
}
