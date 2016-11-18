using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Http;
using SyncthingWeb.Helpers;

namespace SyncthingWeb.Notifications
{
    public class DefaultNotification : INotification
    {
        private const string NotificationMessagesSessionKey = "notifyMessages";

        private readonly ISession session;

        public DefaultNotification(IHttpContextAccessor context)
        {
            this.session = context.HttpContext.Session;
        }

        public IList<NotifyRecord> Notifications
        {
            get
            {
                var records = this.GetCurrentRecords();
                var outputList = new ReadOnlyCollection<NotifyRecord>(new List<NotifyRecord>(records));

                records.Clear();
                return outputList;
            }
        }


        public void Notify(NotifyType type, IconType icon, string format, params object[] args)
        {
            var record = new NotifyRecord { Type = type, Icon = icon, Text = string.Format(format, args) };
            var currList = this.GetCurrentRecords();

            currList?.Add(record);
        }

        public void NotifyInfo(string format, params object[] args)
        {
            this.Notify(NotifyType.Info, IconType.Info, format, args);
        }

        public void NotifyWarn(string format, params object[] args)
        {
            this.Notify(NotifyType.Warning, IconType.Warning, format, args);
        }

        public void NotifyError(string format, params object[] args)
        {
            this.Notify(NotifyType.Danger, IconType.Times, format, args);
        }

        public void NotifySuccess(string format, params object[] args)
        {
            this.Notify(NotifyType.Success, IconType.Check, format, args);
        }

        private IList<NotifyRecord> GetCurrentRecords()
        {
            if (this.session == null)
            {
                return null;
            }

            var collection =
                SessionSerializer.Deserialize<List<NotifyRecord>>(this.session.GetString(NotificationMessagesSessionKey));
            if (collection != null)
            {
                return collection;
            }

            collection = new List<NotifyRecord>();

            this.session.SetString(NotificationMessagesSessionKey, SessionSerializer.Serialize(collection));

            return collection;
        }
    }
}
