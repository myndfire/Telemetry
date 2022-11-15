namespace Application.Monitoring.Common
{
    public class MetricDetails
    {
        public string min { get; set; }
        public string max { get; set; }
        public string value { get; set; }
        public string current { get; set; }

        public override string ToString()
        {
            return $"{nameof(min)}: {min}, {nameof(max)}: {max}, {nameof(value)}: {value}, {nameof(current)}: {current}";
        }
    }
}