using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
                IList<NotifyRecord> records = null;

                GetCurrentRecords(currList =>
                {
                    records = currList.ToList();
                    currList.Clear();
                });

                return records ?? new List<NotifyRecord>();
            }
        }


        public void Notify(NotifyType type, IconType icon, string format, params object[] args)
        {
            var record = new NotifyRecord { Type = type, Icon = icon, Text = string.Format(format, args) };
            GetCurrentRecords(currList => currList.Add(record));


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


        private void GetCurrentRecords(Action<IList<NotifyRecord>> action)
        {
            if (this.session == null)
            {
                return;
            }

            lock (this.session.Id)
            {
                var collection =
                    SessionSerializer.Deserialize<List<NotifyRecord>>(
                        this.session.GetString(NotificationMessagesSessionKey)) ??
                    new List<NotifyRecord>();

                action(collection);
                this.session.SetString(NotificationMessagesSessionKey, SessionSerializer.Serialize(collection));
            }

        }
    }
}
