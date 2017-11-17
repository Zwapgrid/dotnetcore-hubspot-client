using System.Net.Http;
using System.Threading.Tasks;

namespace Skarp.HubSpotClient.Core.Interfaces
{
    public interface IHttpClient
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
    }
}
