using Serilog;
using Serilog.Events;

namespace DatadogApi.Client.Extensions
{
    public static class LogEventHelper
    {
        public static LoggerConfiguration FilterOutEventsWithSensitiveInfo(
            this LoggerConfiguration config) =>
            config.Filter.ByExcluding(UriOrScopeWithApiKey);

        internal static bool UriOrScopeWithApiKey(this LogEvent logEvent)
        {
            if (logEvent.Properties.TryGetValue("Uri", out var uri))
            {
                if ((bool)(uri as ScalarValue)?.Value?.ToString()?.Contains("api_key")!)
                    logEvent.RemovePropertyIfPresent("Uri");
            }

            if (logEvent.Properties.TryGetValue("Scope", out var scope))
            {
                if ((bool)(scope as SequenceValue)?.Elements.Any(x => x.ToString().Contains("api_key"))!)
                    logEvent.RemovePropertyIfPresent("Scope");
            }

            return false;
        }
    }
}