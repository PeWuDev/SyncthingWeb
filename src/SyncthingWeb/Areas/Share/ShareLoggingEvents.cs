using Microsoft.Extensions.Logging;

namespace SyncthingWeb.Areas.Share
{
    public class ShareLoggingEvents
    {
        public static EventId  SharePermission = new EventId(50);
        public static EventId InvalidShareLink = new EventId(51);
    }
}
