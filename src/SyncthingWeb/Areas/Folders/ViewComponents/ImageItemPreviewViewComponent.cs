using System;
using System.IO;
using System.Threading.Tasks;
using ImageSharp;
using ImageSharp.Formats;
using ImageSharp.Processing;
using Microsoft.AspNetCore.Mvc;
using SixLabors.Primitives;
using SyncthingWeb.Areas.Folders.Models;
using SyncthingWeb.Areas.Folders.Services;
using SyncthingWeb.Caching;

namespace SyncthingWeb.Areas.Folders.ViewComponents
{
    public class ImageItemPreviewViewComponent : ViewComponent
    {
        private readonly ICache _cache;
        private readonly ISyncthingFileFetcher _fileFetcher;

        public ImageItemPreviewViewComponent(ISyncthingFileFetcher fileFetcher, ICache cache)
        {
            _fileFetcher = fileFetcher;
            this._cache = cache;
        }

        public async Task<IViewComponentResult> InvokeAsync(PreviewableFileEntryContext context)
        {
            var imageDataUrl = await PreviewImageDataForAsync(context);

            return View(new ImageItemPreviewViewComponentViewModel { Context = context, ImageData = imageDataUrl});
        }

        private async  Task<string> PreviewImageDataForAsync(FileEntryContext context)
        {
            var cacheKey = $"{context.FolderId}-{context.Path}-{context.Name}";
            return await this._cache.GetAsync(cacheKey, async cacheContext =>
            {
                var path =
                    (await _fileFetcher.GetFileToDownloadAsync(context.FolderId,
                        Path.Combine(context.Path, context.Name)))
                    .FullName;
                var imageByteData = Resize(path);
                //TODO scale && cache
                var imageBase64Data = Convert.ToBase64String(imageByteData);
                return string.Format("data:image/png;base64,{0}", imageBase64Data);
            }, TimeSpan.FromMinutes(10));
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
                    }).Save(ms, new JpegEncoder());
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
