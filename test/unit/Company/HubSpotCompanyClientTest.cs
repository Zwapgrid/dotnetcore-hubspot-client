using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NSubstitute;
using Skarp.HubSpotClient.Core;
using Skarp.HubSpotClient.Core.Interfaces;
using Skarp.HubSpotClient.Core.Requests;
using Xunit;
using Xunit.Abstractions;
using Skarp.HubSpotClient.Company;
using Skarp.HubSpotClient.Company.Dto;

namespace Skarp.HubSpotClient.UnitTest.Company
{
    public class HubSpotCompanyClientTest : UnitTestBase<HubSpotCompanyClient>
    {
        private readonly HubSpotCompanyClient _client;
        private readonly IHttpClient _mockHttpClient;
        private readonly RequestSerializer _mockSerializer;

        public HubSpotCompanyClientTest(ITestOutputHelper output) : base(output)
        {
            _mockHttpClient = Substitute.For<IHttpClient>();
            _mockHttpClient.SendAsync(Arg.Any<HttpRequestMessage>())
                .Returns(Task.FromResult(CreateNewEmptyOkResponse()));

            _mockSerializer = Substitute.For<RequestSerializer>();
            _mockSerializer.SerializeEntity(Arg.Any<CompanyHubSpotEntity>()).Returns("{}");
            _mockSerializer.DeserializeEntity<CompanyHubSpotEntity>(Arg.Any<string>()).Returns(new CompanyHubSpotEntity());

            _client = new HubSpotCompanyClient(
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
        [InlineData(HubSpotAction.Create, "/companies/v2/companies")]
        [InlineData(HubSpotAction.Get, "/companies/v2/companies/:companyId:")]
        [InlineData(HubSpotAction.Update, "/companies/v2/companies/:companyId:")]
        [InlineData(HubSpotAction.Delete, "/companies/v2/companies/:companyId:")]
        public void CompanyClient_path_resolver_works(HubSpotAction action, string expetedPath)
        {
            var resvoledPath = _client.PathResolver(new CompanyHubSpotEntity(), action);
            Assert.Equal(expetedPath, resvoledPath);
        }

        [Fact]
        public async Task CompanyClient_create_contact_work()
        {
            var response = await _client.CreateAsync<CompanyHubSpotEntity>(new CompanyHubSpotEntity
            {
                Name = "A new Company",
                Description = "A new description"                
            });

            await _mockHttpClient.Received().SendAsync(Arg.Any<HttpRequestMessage>());
            _mockSerializer.Received().SerializeEntity(Arg.Any<IHubSpotEntity>());
            _mockSerializer.Received().DeserializeEntity<CompanyHubSpotEntity>("{}");
        }

    }
}
