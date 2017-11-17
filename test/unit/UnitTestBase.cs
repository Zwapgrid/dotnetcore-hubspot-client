using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Skarp.HubSpotClient.UnitTest
{
    public abstract class UnitTestBase<T>
    {
        protected readonly ITestOutputHelper Output;
        protected readonly LoggerFactory LoggerFactory;
        protected readonly ILogger<T> Logger;

        protected UnitTestBase(ITestOutputHelper output)
        {
            Output = output;
            LoggerFactory = new LoggerFactory();

            Logger = LoggerFactory.CreateLogger<T>();
        }
    }
}