using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Monitoring.Common
{
    public class DiskMetric
    {
        public string metricId { get; set; }
        public string metricName { get; set; }
        public string frequency { get; set; }
        public IList<Metric> metricValues { get; set; }

        public override string ToString()
        {
            return $"{nameof(metricId)}: {metricId}, {nameof(metricName)}: {metricName}, {nameof(frequency)}: {frequency}, {nameof(metricValues)}: {String.Join(",", metricValues)}";
        }
    }
}
