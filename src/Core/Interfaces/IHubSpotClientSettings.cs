namespace Skarp.HubSpotClient.Core.Interfaces
{
    public interface IHubSpotClientSettings
    {
        string ApiKey { get; }
        string RefreshToken { get; }
        string Code { get; }
        string RedirectUri { get; }
    }
}
