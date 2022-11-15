using System;
using System.IO;
using ClassLibraryA;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Logging.Api;
using Serilog;
using Serilog.Events;

namespace ConsoleAppSerilogElasticsearch
{
    //class ConsoleApplication
    //{
    //    private ILogger<ConsoleApplication> _logger;
    //    public ConsoleApplication(ILogger<ConsoleApplication> logger)
    //    {
    //        _logger = logger;
    //    }
    //    public void ProcessRequest()
    //    {
    //        _logger.LogDebug("ConsoleApplication Running....");
    //    }
    //}
    class Program
    {
        static void Main(string[] args)
        {

           
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            IConfigurationRoot config = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)

                //Will only log the Assembly.FullName log details if the level is Warning or above
                .Enrich.AtLevel(
                    LogEventLevel.Warning,
                    e => e.WithProperty("Version", typeof(Program).Assembly.FullName))
                
                //Will only log the Caller log details if the level is Warning or above
                .Enrich.AtLevel(
                    LogEventLevel.Warning,
                    e => e.WithCaller())
                
                .CreateLogger();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, config);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            //Call method from another namespace
            serviceProvider.GetService<RequestManager>().StartProcessing();

            Log.Information("ConsoleApplication:Starting Application..");

            int a = 10, b = 0;
            try
            {
                Log.Debug("ConsoleApplication:Dividing {A} by {B}", a, b);
                Console.WriteLine(a / b);
            }
            catch (Exception ex)
            {
                ex.Data.Add($"operation={a}/{b}","divide by Zero Error");
                Log.Error(ex, "ConsoleApplication:Something went wrong");
            }
            finally
            {
                Log.CloseAndFlush();
            }
            Console.ReadKey();
        }

        private static void ConfigureServices(ServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddLogging(
                    configure => configure.AddSerilog())
                .AddTransient<RequestManager>()
                .AddTransient<RequestHandler>();
        }
    }
}
