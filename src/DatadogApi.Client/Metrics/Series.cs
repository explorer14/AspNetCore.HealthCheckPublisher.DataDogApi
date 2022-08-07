using System.Collections.Generic;
using System.Linq;

namespace DatadogApi.Client.Metrics
{
    public class Series
    {
        public Series(long posixTimeStamp, int count)
        {
            points.Add(new[] { posixTimeStamp, count }.ToList());
        }

        public string host { get; set; }
        public string metric { get; set; }
        public List<List<long>> points { get; set; } = new List<List<long>>();

        public string tags { get; set; }
        public string type { get; set; }
    }
}