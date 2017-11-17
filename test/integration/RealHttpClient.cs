using System.Net.Http;
using System.Threading.Tasks;
using Skarp.HubSpotClient.Core.Interfaces;

namespace integration
{
    public class RealHttpClient : IHttpClient
    {
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            var client = new HttpClient();
            return await client.SendAsync(request);
        }
    }
}