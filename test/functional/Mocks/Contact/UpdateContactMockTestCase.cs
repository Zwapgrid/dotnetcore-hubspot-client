using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Skarp.HubSpotClient.FunctionalTests.Mocks.Contact
{
    public static class UpdateContactMockTestCase
    {
        public static bool IsMatch(HttpRequestMessage request)
        {
            return request.RequestUri.AbsolutePath.Contains("contacts/v1/contact/vid/2340324/profile") && request.Method == HttpMethod.Post;
        }

        public static Task<HttpResponseMessage> GetResponseAsync(HttpRequestMessage request)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = null,
                StatusCode = HttpStatusCode.NoContent,
                RequestMessage = request
            };


            return Task.FromResult(response);
        }
    }
}