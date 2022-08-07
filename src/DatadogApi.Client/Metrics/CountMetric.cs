using System;
using System.Collections.Generic;

namespace DatadogApi.Client.Metrics
{
    public class CountMetric
    {
        public CountMetric(
            string metricName,
            long posixTimeStamp,
            int count, string tags)
        {
            series.Add(
                new Series(
                    posixTimeStamp: posixTimeStamp,
                    count: count)
                {
                    tags = tags,
                    host = Environment.MachineName,
                    metric = metricName,
                    type = "count"
                });
        }

        public List<Series> series { get; set; } = new List<Series>();
    }
}