using System.Collections.Generic;
using SyncthingWeb.Areas.Folders.Models;

namespace SyncthingWeb.Areas.Folders.Services
{
    public interface IPreviewProviderManager
    {
        IEnumerable<PreviewableFileEntryContext> MakePreviewable(IEnumerable<FileEntryContext> entries);
    }
}
