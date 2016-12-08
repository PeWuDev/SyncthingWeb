using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SyncthingWeb.Areas.Folders.Models;

namespace SyncthingWeb.Areas.Folders.ViewComponents
{
    public class ImageItemPreviewViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(PreviewableFileEntryContext context)
        {
            return Content("hello!");
        }
    }
}
