using System;
using System.IO;
using System.Threading.Tasks;
using ImageSharp;
using ImageSharp.Processing;
using Microsoft.AspNetCore.Mvc;
using SyncthingWeb.Areas.Folders.Models;
using SyncthingWeb.Areas.Folders.Services;

namespace SyncthingWeb.Areas.Folders.ViewComponents
{
    public class ImageItemPreviewViewComponent : ViewComponent
    {
        private readonly ISyncthingFileFetcher _fileFetcher;

        public ImageItemPreviewViewComponent(ISyncthingFileFetcher fileFetcher)
        {
            _fileFetcher = fileFetcher;
        }

        public async Task<IViewComponentResult> InvokeAsync(PreviewableFileEntryContext context)
        {
            var path =
                (await _fileFetcher.GetFileToDownloadAsync(context.FolderId, Path.Combine(context.Path, context.Name)))
                .FullName;
            var imageByteData = Resize(path);
            //TODO scale && cache
            var imageBase64Data = Convert.ToBase64String(imageByteData);
            var imageDataUrl = string.Format("data:image/png;base64,{0}", imageBase64Data);

            return View(new ImageItemPreviewViewComponentViewModel { Context = context, ImageData = imageDataUrl});
        }


        private static byte[] Resize(string file)
        {
            using (var fileStream = File.OpenRead(file))
            {
                return Resize(fileStream);
            }
        }

        private static byte[] Resize(Stream file)
        {
            using (var ms = new MemoryStream())
            {
                using (var image = Image.Load(Configuration.Default, file))
                {
                    image.Resize(new ResizeOptions
                    {
                        Size = new Size(90, 90),
                        Mode = ResizeMode.Crop,
                        Position = AnchorPosition.Center
                    }).Save(ms);
                }

                ms.Position = 0;
                return ms.ToArray();
            }
        }

        public class ImageItemPreviewViewComponentViewModel
        {
            public PreviewableFileEntryContext Context { get; set; }
            public string ImageData { get; set; }
        }
    }
}
