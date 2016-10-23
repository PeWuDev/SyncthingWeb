using Newtonsoft.Json;

namespace SyncthingWeb.Helpers
{
    public class SessionSerializer
    {
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T Deserialize<T>(string cacheValue)
        {
            if (string.IsNullOrEmpty(cacheValue)) return default(T);

            return JsonConvert.DeserializeObject<T>(cacheValue);
        }
    }
}
