using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace SampleOpenTelemetryTwo.Services
{
    public class MyMetrics: IDisposable
    {
        private Meter _meter;
        private ActivityListener _listener;
        private Histogram<double> _histogram;

        public MyMetrics()
        {
            _meter = Measures.GetMeter(Measures.InstrumentationName); 
            _histogram = _meter.CreateHistogram<double>(name: "duration", unit: "ms");
            _listener = new ActivityListener
            {
                ShouldListenTo = x => x.Name == Measures.InstrumentationName,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
                ActivityStopped = ActivityStopped
            };
            ActivitySource.AddActivityListener(_listener);


        }

        private void ActivityStopped(Activity activity)
        {
            _histogram.Record(activity.Duration.TotalMilliseconds);
        }

        public void Dispose()
        {
            _meter.Dispose();
            _listener.Dispose();
        }
    }
}
