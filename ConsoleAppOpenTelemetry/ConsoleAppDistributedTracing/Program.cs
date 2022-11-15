using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace ConsoleAppDistributedTracing
{
    internal class Program
    {
        private static ActivitySource source = new ActivitySource("ConsoleAppDistributedTracing", "1.0.0");
        static string zipkinUrl = "http://127.0.0.1:9411/api/v2/spans";

        private static async Task  Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            using var tracerProvider = Sdk.CreateTracerProviderBuilder()
                .AddSource("Sample.Console")
                .AddHttpClientInstrumentation()
                .AddConsoleExporter()
                .AddZipkinExporter(options => options.Endpoint = new Uri(zipkinUrl))
                .Build();

            bool endApp = false;

            Console.WriteLine("Distributed Tracing Example - Console Client");
            Console.WriteLine("-----------------------");

            while (!endApp)
            {
                Console.WriteLine("Hit any key to send HttpClient request.");
                Console.ReadKey();

                await CallWebEndpoint();

                // Wait for the user to respond before closing.
                Console.Write("Press 'n' and Enter to close the app, or press any other key and Enter to continue: ");
                if (Console.ReadLine() == "n") endApp = true;

            }
            return ;
        }

        private static async Task CallWebEndpoint()
        {
            using (Activity activity = source.StartActivity("CallWebEndpoint", ActivityKind.Client)) // Span
            {

                string url = "http://localhost:5000/weatherforecast";
                activity?.SetTag("URL", url);

                using (var client = new HttpClient())
                {

                    //activity?.AddBaggage("Originating Computer", Environment.MachineName);

                    //activity?.AddEvent(new ActivityEvent("Call to WeatherForecast API starting."));

                    var postResponse = await client.GetAsync(url);
                    postResponse.EnsureSuccessStatusCode();

                    //activity?.AddEvent(new ActivityEvent("Call to WeatherForecast API completed."));

                }
            }
        }
    }
}
