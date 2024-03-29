﻿namespace AspNetCore.HealthCheckPublisher.DataDogApi.Metrics
{
    public class GuageMetric
    {
        public GuageMetric(
            string metricName,
            long posixTimeStamp,
            int value, List<string>? tags)
        {
            series.Add(
                new Series(
                    posixTimeStamp: posixTimeStamp,
                    count: value,
                    hostName: Environment.MachineName)
                {
                    tags = tags ?? Enumerable.Empty<string>().ToList(),
                    metric = metricName,
                    type = 3
                });
        }

        public List<Series> series { get; set; } = new List<Series>();
    }
}