namespace AspNetCore.HealthCheckPublisher.DataDogApi.Metrics
{
    public class Point
    {
        public long timestamp { get; set; }

        public double value { get; set; }
    }

    public class Resource
    {
        public string name { get; set; }
        public string type { get; set; }
    }

    public class Series
    {
        public Series(long posixTimeStamp, int count, string hostName)
        {
            points.Add(new Point { timestamp = posixTimeStamp, value = count });
            resources.Add(new Resource { type = "host", name = hostName });
        }

        public string metric { get; set; }

        public List<Point> points { get; set; } = new List<Point>();

        public List<Resource> resources { get; set; } = new List<Resource>();

        public List<string> tags { get; set; } = new List<string>();

        public int type { get; set; }
    }
}