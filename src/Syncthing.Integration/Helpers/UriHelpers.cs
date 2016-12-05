using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Net;

namespace Syncthing.Integration.Helpers
{
    public class UriHelpers
    {
        public static string GetQueryString(object obj)
        {
            if (obj == null) return string.Empty;

            var result = new List<string>();
            var props = obj.GetType().GetTypeInfo().GetProperties().Where(p => p.GetValue(obj, null) != null);

            foreach (var p in props)
            {
                var value = p.GetValue(obj, null);
                var enumerable = value as ICollection;
                if (enumerable != null)
                {
                    result.AddRange(from object v in enumerable select string.Format("{0}={1}", p.Name, WebUtility.UrlEncode(v.ToString())));
                }
                else
                {
                    result.Add(string.Format("{0}={1}", p.Name, WebUtility.UrlEncode(value.ToString())));
                }
            }

            return string.Join("&", result.ToArray());
        }
    }
}
