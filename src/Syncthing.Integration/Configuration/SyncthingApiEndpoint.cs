using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                var response = await httpClient.GetStringAsync(BuildUrl(uri, uriParams));
                return response;
            }
        }

        public async Task<dynamic> GetDynamicDataAsync(string uri, object uriParams = null)
        {
            using (var httpClient = this.GetClient())
            {
                var response = await httpClient.GetStringAsync(BuildUrl(uri, uriParams));
                return JObject.Parse(response);
            }
        }

        public async Task<T> GetDataAsync<T>(string uri, object uriParams = null)
        {
            using (var httpClient = this.GetClient())
            {
                var response = await httpClient.GetStringAsync(BuildUrl(uri, uriParams));
                return JsonConvert.DeserializeObject<T>(response);
            }
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
