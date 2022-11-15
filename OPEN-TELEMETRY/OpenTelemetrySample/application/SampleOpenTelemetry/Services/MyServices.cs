using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace SampleOpenTelemetry.Services
{

    public class MyServices:IMyServices
    {
        private MyTraces _myTraces;
        private static Meter meter;
        private static Counter<int> PingRequestCounter;
        private static Counter<int> PongRequestCounter;

        private HttpClient httpClient;
        ILogger<MyServices> _logger;


        public MyServices(ILogger<MyServices> logger, MyTraces myTraces)
        {
            meter = Measures.GetMeter(Measures.InstrumentationName);
            PingRequestCounter = meter.CreateCounter<int>("ping");
            PongRequestCounter = meter.CreateCounter<int>("pong");

            httpClient = new HttpClient();
            _logger = logger;
            _myTraces=myTraces;
        }
        public async Task<bool> Ping()
        {
            PingRequestCounter.Add(1);
            using var activity = _myTraces.Source.StartActivity(); // Can add listeners to activity.Events()

            activity?.AddTag("CustomTag", "Ping()");
            activity?.AddBaggage("CustomBaggage", "customTraceBaggage");
            var str1 = await httpClient.GetStringAsync("https://example.com");
            var str2 = await httpClient.GetStringAsync("https://www.yahoo.net");
            //var str3 = await httpClient.GetStringAsync("http://localhost:5120");
            await Pong();
            // await httpClient.GetStringAsync("https://localhost:7259/compute"); // Call downstream service

            _logger.LogInformation("example.com response length: {Length}", str1.Length);
            _logger.LogInformation("yahoo.net response length: {Length}", str2.Length);

            return  true;
        }

        public async Task Pong()
        {
            Random _random = new Random();
            PongRequestCounter.Add(1);

            using var activity = _myTraces.Source.StartActivity(); // Can add listeners to activity.Events()
            //throw new Exception("Pong Error!!");
            activity?.SetTag("foo-int", 1);
            activity?.SetTag("bar-string", "Hello, World!");
            activity?.SetTag("foobar-object", new int[] { 1, 2, 3 });
            activity?.AddEvent(new ActivityEvent("Pong Log: this is a log attached to span"));

            var delay = _random.Next(100, 200);
            await Task.Delay(delay);

        }
    }
}
