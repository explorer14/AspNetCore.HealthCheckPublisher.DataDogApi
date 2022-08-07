using System;
using System.Collections.Generic;
using System.Text;

namespace DatadogApi.Client.Metrics
{
    public class GuageMetric
    {
        public GuageMetric(
            string metricName,
            long posixTimeStamp,
            int value, string tags)
        {
            series.Add(
                new Series(
                    posixTimeStamp: posixTimeStamp,
                    count: value)
                {
                    tags = tags,
                    host = Environment.MachineName,
                    metric = metricName,
                    type = "guage"
                });
        }

        public List<Series> series { get; set; } = new List<Series>();
    }
}