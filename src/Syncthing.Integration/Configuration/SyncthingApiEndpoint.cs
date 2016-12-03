namespace Syncthing.Integration.Configuration
{
    public class SyncthingApiEndpoint
    {
        public SyncthingApiEndpoint(string apiKey, string endpoint)
        {
            this.ApiKey = apiKey;
            this.Endpoint = endpoint;
        }

        public string ApiKey { get; }

        public string Endpoint { get; }
    }
}
