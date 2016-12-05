using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Syncthing.Integration.Helpers
{
    internal class JsonContent : StringContent
    {
        private const string ContentType = "application/json";

        public JsonContent(object obj) :
            base(JsonConvert.SerializeObject(obj), Encoding.UTF8, ContentType)
        {
        }
    }
}
