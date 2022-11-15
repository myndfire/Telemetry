using System.Diagnostics.Metrics;
using System.Reflection;

namespace SampleOpenTelemetry.Services
{
    public static class Measures
    {
        static readonly AssemblyName _assemblyName=typeof(MyServices).Assembly.GetName();
        private static readonly Version? _version = _assemblyName.Version;
        public const string InstrumentationName = "MyServices";
        public static Meter GetMeter(string metricsName) => new(metricsName, _assemblyName?.ToString());
    }
}
