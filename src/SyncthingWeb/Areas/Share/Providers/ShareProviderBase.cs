using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using SyncthingWeb.Areas.Share.Providers.Implementation;
using SyncthingWeb.Bus;

namespace SyncthingWeb.Areas.Share.Providers
{
    public interface IShare
    {
        Guid Id { get; }

        string Title { get; }
        string Decription { get; }

        string IconCss { get; }

        string AreaOfController { get; }
        string NameOfController { get; }

        string ConfigureLink(ActionContext actionContext, string id, string path);

    }

    public interface IShareCollector
    {
        void Add(IShare share);
    }

    public class ShareCollector : IShareCollector
    {
        private readonly IList<IShare> shares = new List<IShare>();

        public IEnumerable<IShare> Shares => new ReadOnlyCollection<IShare>(shares);

        public void Add(IShare share)
        {
            this.shares.Add(share);
        }
    }

    public class ShareProvider : IEventHandler<IShareCollector>
    {
        public Task HandleAsync(IShareCollector @event)
        {
            @event.Add(new PasswordProtectedShareProvider());
            @event.Add(new PublicShareProvider());

            return Task.FromResult(0);
        }
    }

    public abstract class ShareProviderBase : IShare
    {

        public abstract Guid Id { get; }
        public abstract string Title { get; }
        public abstract string Decription { get; }
        public abstract string IconCss { get; }

        public abstract string AreaOfController { get; }
        public abstract string NameOfController { get; }        

        public string ConfigureLink(ActionContext actionContext, string id, string path)
        {            
            var urlHelper = new UrlHelper(actionContext);
            return urlHelper.Action("Configure", this.NameOfController,
                new {area = this.AreaOfController, id, path});
        }
    }
}