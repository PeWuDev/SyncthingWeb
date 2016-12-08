using System.Collections.Generic;
using System.Linq;
using SyncthingWeb.Areas.Folders.Models;
using SyncthingWeb.Areas.Folders.Preview;
using SyncthingWeb.Areas.Folders.Preview.Providers;

namespace SyncthingWeb.Areas.Folders.Services
{
    public class DefaultPreviewProviderManager : IPreviewProviderManager
    {
        private HashSet<PreviewProvider> providers;

        public DefaultPreviewProviderManager()
        {
            this.providers = new HashSet<PreviewProvider>
            {
                new ImagePreviewProvider()
            };
        }

        public IEnumerable<PreviewableFileEntryContext> MakePreviewable(IEnumerable<FileEntryContext> entries)
        {
            foreach (var entry in entries)
            {
                var provider = providers.FirstOrDefault(pr => pr.CanPreview(entry));
                yield return new PreviewableFileEntryContext(entry, provider);
            }
        }
    }
}
