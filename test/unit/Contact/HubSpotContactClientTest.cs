using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NSubstitute;
using Skarp.HubSpotClient.Contact;
using Skarp.HubSpotClient.Contact.Dto;
using Skarp.HubSpotClient.Core;
using Skarp.HubSpotClient.Core.Interfaces;
using Skarp.HubSpotClient.Core.Requests;
using Xunit;
using Xunit.Abstractions;

namespace Skarp.HubSpotClient.UnitTest.Contact
{
    public class HubSpotContactClientTest : UnitTestBase<HubSpotContactClient>
    {
        private readonly HubSpotContactClient _client;
        private readonly IHttpClient _mockHttpClient;
        private readonly RequestSerializer _mockSerializer;

        public HubSpotContactClientTest(ITestOutputHelper output) : base(output)
        {
            _mockHttpClient = Substitute.For<IHttpClient>();
            _mockHttpClient.SendAsync(Arg.Any<HttpRequestMessage>())
                .Returns(Task.FromResult(CreateNewEmptyOkResponse()));

            _mockSerializer = Substitute.For<RequestSerializer>();
            _mockSerializer.SerializeEntity(Arg.Any<ContactHubSpotEntity>()).Returns("{}");
            _mockSerializer.DeserializeEntity<ContactHubSpotEntity>(Arg.Any<string>()).Returns(new ContactHubSpotEntity());
            _mockSerializer.DeserializeListEntity<ContactListHubSpotEntity<ContactHubSpotEntity>>(Arg.Any<string>()).Returns(new ContactListHubSpotEntity<ContactHubSpotEntity>());
            
            _client = new HubSpotContactClient(
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
        [InlineData(HubSpotAction.Create, "/contacts/v1/contact")]
        [InlineData(HubSpotAction.Get, "/contacts/v1/contact/vid/:contactId:/profile")]
        [InlineData(HubSpotAction.List, "/contacts/v1/lists/all/contacts/all")]
        [InlineData(HubSpotAction.Update, "/contacts/v1/contact/vid/:contactId:/profile")]
        [InlineData(HubSpotAction.Delete, "/contacts/v1/contact/vid/:contactId:")]
        public void ContactClient_path_resolver_works(HubSpotAction action, string expetedPath)
        {
            var resvoledPath = _client.PathResolver(new ContactHubSpotEntity(), action);
            Assert.Equal(expetedPath, resvoledPath);
        }

        [Fact]
        public async Task ContactClient_create_contact_work()
        {
            var response = await _client.CreateAsync<ContactHubSpotEntity>(new ContactHubSpotEntity
            {
                FirstName = "Adrian",
                Lastname = "Baws",
                Email = "adrian@the-email.com"
            });

            await _mockHttpClient.Received().SendAsync(Arg.Any<HttpRequestMessage>());
            _mockSerializer.Received().SerializeEntity(Arg.Any<IHubSpotEntity>());
            _mockSerializer.Received().DeserializeEntity<ContactHubSpotEntity>("{}");
        }

        [Fact]
        public async Task ContactClient_list_contacts_work()
        {
            var response = await _client.ListAsync<ContactListHubSpotEntity<ContactHubSpotEntity>>();

            await _mockHttpClient.Received().SendAsync(Arg.Any<HttpRequestMessage>());
            _mockSerializer.Received().DeserializeListEntity<ContactListHubSpotEntity<ContactHubSpotEntity>>("{}");
        }
    }
}
