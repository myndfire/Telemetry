using System.Diagnostics;
using System.Diagnostics.Metrics;
using Azure.Messaging.ServiceBus;

namespace SampleOpenTelemetryTwo.Services
{

    public class MyServices : IMyServices
    {
        private MyTraces _myTraces;
        private static Meter meter;
        private static Counter<int> FooRequestCounter;
        private static Counter<int> BarRequestCounter;

        private HttpClient httpClient;
        ILogger<MyServices> _logger;


        public MyServices(ILogger<MyServices> logger, MyTraces myTraces)
        {
            meter = Measures.GetMeter(Measures.InstrumentationName);
            FooRequestCounter = meter.CreateCounter<int>("foo");
            BarRequestCounter = meter.CreateCounter<int>("bar");

            httpClient = new HttpClient();
            _logger = logger;
            _myTraces = myTraces;
        }

        public async Task<bool> Foo()
        {
            FooRequestCounter.Add(1);
            using var activity = _myTraces.Source.StartActivity(); // Can add listeners to activity.Events()

            activity?.AddTag("CustomTag", "Foo()");
            activity?.AddBaggage("CustomBaggage", "customTraceBaggage");
            var str1 = await httpClient.GetStringAsync("https://example.com");
            var str2 = await httpClient.GetStringAsync("https://www.yahoo.net");
            await Bar();
            _logger.LogInformation("example.com response length: {Length}", str1.Length);
            _logger.LogInformation("yahoo.net response length: {Length}", str2.Length);

            return true;
        }

        public async Task Bar()
        {
            Random _random = new Random();
            BarRequestCounter.Add(1);

            using var activity = _myTraces.Source.StartActivity(); // Can add listeners to activity.Events()
            //throw new Exception("Bar Error!!");
            activity?.SetTag("foo-int", 1);
            activity?.SetTag("bar-string", "Hello, World!");
            activity?.SetTag("foobar-object", new int[] {1, 2, 3});
            activity?.AddEvent(new ActivityEvent("Bar Log: this is a log attached to span"));

            var delay = _random.Next(100, 200);
            await Task.Delay(delay);

        }

        public async Task PublishMessage()
        {
            using var activity = _myTraces.Source.StartActivity(); // Can add listeners to activity.Events()
            activity?.AddEvent(new ActivityEvent("PublishMessage: started()"));
            // connection string to your Service Bus namespace
            string connectionString =
                "Endpoint=sb://mhcsandbox.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=utaK0lS+xir46EzRUFU1oyjo5wSIuyGFpjq2M0sk418=";

            // name of your Service Bus queue
            string queueName = "commandrequests";


            // the client that owns the connection and can be used to create senders and receivers
            ServiceBusClient client;

            // the sender used to publish messages to the queue
            ServiceBusSender sender;

            // the processor that reads and processes messages from the queue
            ServiceBusProcessor processor;

        // number of messages to be sent to the queue
         int numOfMessages = 3;

         // Create the clients that we'll use for sending and processing messages.
         client = new ServiceBusClient(connectionString);
         sender = client.CreateSender(queueName);


            // create a batch 
            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            for (int i = 1; i <= numOfMessages; i++)
            {
                // try adding a message to the batch
                var message = new ServiceBusMessage($"Message {i}");
                message.ApplicationProperties.Add("trace_id", activity.Context.TraceId.ToString());
                message.ApplicationProperties.Add("span_id", activity.Context.SpanId.ToString());


                message.CorrelationId = activity.Context.TraceId.ToString();
                Console.WriteLine($"--- Publisher().Message.CorrelationId = {message.CorrelationId}  --------");
                if (!messageBatch.TryAddMessage(message))
                {
                    // if it is too large for the batch
                    throw new Exception($"The message {i} is too large to fit in the batch.");
                }
            }

            try
            {
                // Use the producer client to send the batch of messages to the Service Bus queue
                await sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A batch of {numOfMessages} messages has been published to the queue.");
            }
            finally
            {
                // Calling DisposeAsync on client types is required to ensure that network
                // resources and other unmanaged objects are properly cleaned up.
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }
            activity?.AddEvent(new ActivityEvent("PublishMessage: completed()"));
        }
    }
}
