using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Skarp.HubSpotClient.FunctionalTests
{
    public abstract class FunctionalTestBase<T>
    {
        protected readonly ITestOutputHelper Output;
        protected readonly LoggerFactory LoggerFactory;
        protected readonly ILogger<T> Logger;

        protected FunctionalTestBase(ITestOutputHelper output)
        {
            Output = output;
            LoggerFactory = new LoggerFactory();

            Logger = LoggerFactory.CreateLogger<T>();
        }
    }
}
