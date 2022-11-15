using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Reflection;
using Azure.Messaging.ServiceBus;
using ConsoleAppQueueProcessor.Services;
using Grpc.Core;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ConsoleAppQueueProcessor
{
    class Program
    {
        private static MyTraces _myTraces = new MyTraces();
        private static Meter meter;
        private static Counter<int> ConsoleAppQueueProcessorCounter;

        // connection string to your Service Bus namespace
        static string connectionString = "Endpoint=sb://mhcsandbox.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=utaK0lS+xir46EzRUFU1oyjo5wSIuyGFpjq2M0sk418=";

        // name of your Service Bus queue
        static string queueName = "commandrequests";

        // the client that owns the connection and can be used to create senders and receivers
        static ServiceBusClient client;

        // the sender used to publish messages to the queue
        static ServiceBusSender sender;

        // the processor that reads and processes messages from the queue
        static ServiceBusProcessor processor;

        // number of messages to be sent to the queue
        private const int numOfMessages = 3;



        private static readonly ActivitySource MyActivitySource = new ActivitySource("ConsoleAppQueueProcessor");

        static async Task Main()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);

            //--------------  Tracing --------------------
            var env = "Development";
            var applicationName = "ConsoleAppQueueProcessor";
            var assemblyVersion = Assembly
                .GetExecutingAssembly()
                .GetCustomAttribute<AssemblyFileVersionAttribute>()!
                .Version;

            meter = Measures.GetMeter(Measures.InstrumentationName);
            ConsoleAppQueueProcessorCounter = meter.CreateCounter<int>("ConsoleAppQueueProcessor");



            using var openTelemetry = Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(
                    ResourceBuilder
                        .CreateDefault()
                        //.CreateEmpty()
                        .AddService(serviceName: applicationName, serviceVersion: applicationName)
                        .AddAttributes(
                            new KeyValuePair<string, object>[]
                            {
                                new("deployment.environment",env),
                                new("host.name", Environment.MachineName),
                                new("application.name", applicationName),
                                new("application.assemblyversion",assemblyVersion)
                            })
                        .AddEnvironmentVariableDetector() //Set using powershell: $env:OTEL_RESOURCE_ATTRIBUTES = 'key1=value1,key2=value2'
                )

                .SetSampler(new AlwaysOnSampler())
                .AddAspNetCoreInstrumentation(
                    options =>
                    {
                        //options.Enrich = Enrich;
                        options.RecordException = true;
                    })
                .AddHttpClientInstrumentation()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri("http://localhost:4317");
                        //options.ExportProcessorType = ExportProcessorType.Simple;
                    })
                    .AddConsoleExporter(options => options.Targets = ConsoleExporterOutputTargets.Console)
                    .AddInstrumentation<MyTraces>()
                    .AddSource(Measures.InstrumentationName)
                .AddSource("Azure.*")
                .Build();





            // The Service Bus client types are safe to cache and use as a singleton for the lifetime
            // of the application, which is best practice when messages are being published or read
            // regularly.
            //
            // Create the clients that we'll use for sending and processing messages.
            client = new ServiceBusClient(connectionString);
            //sender = client.CreateSender(queueName);
            await Receiver();
            Console.WriteLine("Press any key to end the application");
            Console.ReadKey();
        }

        static async Task Receiver()
        {
            // The Service Bus client types are safe to cache and use as a singleton for the lifetime
            // of the application, which is best practice when messages are being published or read
            // regularly.
            //

            // Create the client object that will be used to create sender and receiver objects
            client = new ServiceBusClient(connectionString);

            // create a processor that we can use to process the messages
            processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

            try
            {
                // add handler to process messages
                processor.ProcessMessageAsync += MessageHandler;

                // add handler to process any errors
                processor.ProcessErrorAsync += ErrorHandler;

                // start processing 
                await processor.StartProcessingAsync();

                Console.WriteLine("Wait for a minute and then press any key to end the processing");
                Console.ReadKey();

                // stop processing 
                Console.WriteLine("\nStopping the receiver...");
                await processor.StopProcessingAsync();
                Console.WriteLine("Stopped receiving messages");
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await processor.DisposeAsync();
                await client.DisposeAsync();
            }
        }


        // handle received messages
        static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            ServiceBusReceivedMessage message = args.Message;
            
            ConsoleAppQueueProcessorCounter.Add(1);
            using var activity = _myTraces.Source.StartActivity(); // Can add listeners to activity.Events()
            if (message.ApplicationProperties.TryGetValue("Diagnostic-Id", out var objectId) && objectId is string diagnosticId)
            {
                Console.WriteLine($"----- Diagnostic-Id = {diagnosticId}");
                if (message.ApplicationProperties.TryGetValue("span_id", out var spanId) && spanId is string spanid)
                {
                    Console.WriteLine($"----- spanid = {spanid}");
                    //activity.SetParentId(spanid);

                }
                if (message.ApplicationProperties.TryGetValue("trace_id", out var traceId) && traceId is string traceid)
                {
                    Console.WriteLine($"----- traceid = {traceid}");
                    activity.SetParentId(traceid);
                    

                }
                //activity.SetParentId(diagnosticId);

            }
            activity?.AddEvent(new ActivityEvent("MessageHandler started"));

            activity?.AddTag("ConsoleAppQueueProcessorCounter", "Foo()");

            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body}");

            // complete the message. message is deleted from the queue. 
            await args.CompleteMessageAsync(args.Message);
            activity?.AddEvent(new ActivityEvent("MessageHandler completed"));

        }

        // handle any errors when receiving messages
        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

    }
}