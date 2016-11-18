using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SyncthingWeb.Notifications;

namespace SyncthingWeb.ViewComponents
{
    public class NotificationViewComponent : ViewComponent
    {
        private readonly INotification notification;

        public NotificationViewComponent(INotification notification)
        {
            this.notification = notification;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var all = this.notification.Notifications;

            return View(all);
        }
    }
}
