using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NSubstitute;
using Skarp.HubSpotClient.Core;
using Skarp.HubSpotClient.Core.Interfaces;
using Skarp.HubSpotClient.Core.Requests;
using Skarp.HubSpotClient.Deal;
using Skarp.HubSpotClient.Deal.Dto;
using Xunit;
using Xunit.Abstractions;

namespace Skarp.HubSpotClient.UnitTest.Deal
{
    public class HubSpotDealClientTest : UnitTestBase<HubSpotDealClient>
    {
        private readonly HubSpotDealClient _client;
        private readonly IHttpClient _mockHttpClient;
        private readonly RequestSerializer _mockSerializer;

        public HubSpotDealClientTest(ITestOutputHelper output) : base(output)
        {
            _mockHttpClient = Substitute.For<IHttpClient>();

            _mockHttpClient.SendAsync(Arg.Any<HttpRequestMessage>())
                .Returns(Task.FromResult(CreateNewEmptyOkResponse()));

            _mockSerializer = Substitute.For<RequestSerializer>();
            _mockSerializer.SerializeEntity(Arg.Any<DealHubSpotEntity>()).Returns("{}");
            _mockSerializer.DeserializeEntity<DealHubSpotEntity>(Arg.Any<string>()).Returns(new DealHubSpotEntity());

            _client = new HubSpotDealClient(
                _mockHttpClient,
                Logger,
                _mockSerializer,
                "https://api.hubapi.com",
                "HapiKeyFisk"
                );
        }

        private HttpResponseMessage CreateNewEmptyOkResponse()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new JsonContent("{}")
            };
            return response;
        }

        [Theory]
        [InlineData(HubSpotAction.Create, "/deals/v1/deal")]
        [InlineData(HubSpotAction.Get, "/deals/v1/deal/:dealId:")]
        [InlineData(HubSpotAction.Update, "/deals/v1/deal/:dealId:")]
        [InlineData(HubSpotAction.Delete, "/deals/v1/deal/:dealId:")]
        public void DealClient_path_resolver_works(HubSpotAction action, string expetedPath)
        {
            var resvoledPath = _client.PathResolver(new DealHubSpotEntity(), action);
            Assert.Equal(expetedPath, resvoledPath);
        }

        [Fact]
        public async Task DealClient_create_contact_work()
        {
            var response = await _client.CreateAsync<DealHubSpotEntity>(new DealHubSpotEntity
            {
                Name = "A new deal",
                Pipeline = "default",
                Amount = 60000,
                DealType = "newbusiness"
            });

            await _mockHttpClient.Received().SendAsync(Arg.Any<HttpRequestMessage>());
            _mockSerializer.Received().SerializeEntity(Arg.Any<IHubSpotEntity>());
            _mockSerializer.Received().DeserializeEntity<DealHubSpotEntity>("{}");
        }

    }
}
