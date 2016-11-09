using System.Collections.Generic;

namespace SyncthingWeb.Notifications
{
    public interface INotification
    {
        IList<NotifyRecord> Notifications { get; }

        void Notify(NotifyType type, IconType icon, string format, params object[] args);
        void NotifyInfo(string format, params object[] args);
        void NotifyWarn(string format, params object[] args);
        void NotifyError(string format, params object[] args);
        void NotifySuccess(string format, params object[] args);
    }

    public class NotifyRecord
    {
        public NotifyType Type { get; set; }
        public IconType Icon { get; set; }
        public string Text { get; set; }
    }

    public enum NotifyType
    {
        Info = 0,
        Warning = 1,
        Danger = 2,
        Success = 3
    }

    public enum IconType
    {
        Warning = 0,
        Check = 1,
        Info = 2,
        Times = 3
    }
}
