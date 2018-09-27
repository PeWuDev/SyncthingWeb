using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Syncthing.Integration.Exceptions;
using Syncthing.Integration.Helpers;

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

        public async Task<string> GetRawJsonDataAsync(string uri, object uriParams = null)
        {
            using (var httpClient = this.GetClient())
            {
                string response = null;

                try
                {
                    response = await httpClient.GetStringAsync(BuildUrl(uri, uriParams));
                }
                catch (Exception ex)
                {
                    HandleConnectionError(ex);
                }

                return response;
            }
        }

        public async Task<dynamic> GetDynamicDataAsync(string uri, object uriParams = null)
        {
            var response = await GetRawJsonDataAsync(uri, uriParams);
            return JObject.Parse(response);
        }

        public async Task<T> GetDataAsync<T>(string uri, object uriParams = null)
        {
            var response = await GetRawJsonDataAsync(uri, uriParams);
            return JsonConvert.DeserializeObject<T>(response);
        }
        private static void HandleConnectionError(Exception ex)
        {
            throw new SyncthingConnectionException(
                $"Error while connecting to Syncthing endpoint: {ex.InnerException?.Message ?? ex.Message ?? "no message available."}",
                ex.InnerException ?? ex);
        }

        private string BuildUrl(string uri, object uriParams)
        {
            if (uri.Contains("?")) throw new NotSupportedException("Custom url paramters not supported yet.");

            uri = this.Endpoint.TrimEnd('/') + "/" + uri.Trim('/');

            var parameters = UriHelpers.GetQueryString(uriParams);
            return string.IsNullOrWhiteSpace(parameters) ? uri : $"{uri}?{parameters}";
        }

        private HttpClient GetClient()
        {
            var cl = new HttpClient();
            cl.DefaultRequestHeaders.Add("X-API-Key", this.ApiKey);

            return cl;
        }
    }
}
