using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Prometheus;

namespace WebApplicationPrometheus
{

    public class WeatherForecastMetricsCollector
    {
        //------ Counter Metric
        // (will be affected by restarts)
        //sum(weatherforecast_summarytype_counter ) by (Weather) // Returns individual counts grouped by Weather
        //sum(weatherforecast_summa dvxvgzerytype_counter) // Returns Sum of all the counts for Cold, Pleasant, Hot as one value
        //sum(weatherforecast_summarytype_counter {Weather="Cold"})
        //sum(weatherforecast_summarytype_counter ) by (service) // Can get the sum from different services like instances running in Dev/Cert/Prod



        //------- Histogram Metrics
        //rate(weatherforecast_temp_histogram_bucket{Weather="Cold"}[30m])
        //rate(weatherforecast_temp_histogram_bucket{Weather="Cold", le="0"}[30m])

        //90th percentile
        //histogram_quantile(0.90, rate(weatherforecast_temp_histogram_bucket{ Weather = "Cold"}[90m]))
        //histogram_quantile(0.9, rate(weatherforecast_temp_histogram_bucket[90m]))
        // weatherforecast_temp_histogram_sum{Weather="Cold"}
        private readonly ILogger<WeatherForecastMetricsCollector> _logger;

        private readonly Histogram _WeatherForecastTemperatureHistogram;
        private readonly Counter _WeatherForecastSummaryTypeCounter;
        private readonly Gauge _WeatherForecastTemperatureGauge;
        public WeatherForecastMetricsCollector(ILogger<WeatherForecastMetricsCollector> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _WeatherForecastSummaryTypeCounter = Metrics.CreateCounter("weatherforecast_summarytype_counter", "Counter by each Summary Type", 
                                                                        new CounterConfiguration
                                                                        {
                                                                            LabelNames = new[] { "ApplicationId", "Weather" }
                                                                        });
            _WeatherForecastTemperatureGauge = Metrics.CreateGauge("weatherforecast_temp_gauge", "weatherforecast temp gauge",
                                                                    new GaugeConfiguration
                                                                    {
                                                                        LabelNames = new[] { "ApplicationId", "Weather" }
                                                                    });
            _WeatherForecastTemperatureHistogram = Metrics.CreateHistogram("weatherforecast_temp_histogram", 
                                                                           "weatherforecast temperature histogram by category", 
                                                                            new HistogramConfiguration
                                                                            {
                                                                                //Buckets = Histogram.ExponentialBuckets(0.01, 2, 10),
                                                                                //Buckets = Histogram.LinearBuckets(-20, 10, 6),
                                                                                Buckets = new[] { -20.0, 0, 20, 40, 60 },
                                                                                LabelNames = new[] { "ApplicationId","Weather" }
                                                                            });

            //_WeatherForecastTemperatureGauge.Set(0);
        }

        public void TemperatureSummaryTypeCounterMetricIncrement(string applicationid, string summarytype)
        {
            _WeatherForecastSummaryTypeCounter.Labels(applicationid, summarytype).Inc();
        }
        public void TemperatureGaugeMetricSetvalue(string applicationid, string summarytype, double temp)
        {

            _WeatherForecastTemperatureGauge.Labels(applicationid, summarytype).Set(temp);
            //_WeatherForecastTemperatureGauge.Set(temp);
        }
        public void TemperatureHistogramMetricSetSummaryTypeValue(string applicationid, string bucket, double value)
        {
            _WeatherForecastTemperatureHistogram.Labels(applicationid,bucket).Observe(value);
        }
    }
}

