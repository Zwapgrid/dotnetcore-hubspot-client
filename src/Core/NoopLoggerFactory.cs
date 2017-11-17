using Microsoft.Extensions.Logging;

namespace Skarp.HubSpotClient.Core
{
    public static class NoopLoggerFactory
    {
        public static ILogger Get()
        {
            return new NoopLogger<HubSpotAction>();
        }

        public static ILogger<T> Get<T>()
        {
            return new NoopLogger<T>();
        }
    }
}
