namespace AspNetCore.HealthCheckPublisher.DataDogApi.Metrics
{
    public class CountMetric
    {
        public CountMetric(
            string metricName,
            long posixTimeStamp,
            int count, List<string>? tags)
        {
            series.Add(
                new Series(
                    posixTimeStamp: posixTimeStamp,
                    count: count,
                    hostName: Environment.MachineName)
                {
                    tags = tags ?? Enumerable.Empty<string>().ToList(),
                    metric = metricName,
                    type = 1
                });
        }

        public List<Series> series { get; set; } = new List<Series>();
    }
}