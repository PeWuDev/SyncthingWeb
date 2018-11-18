using Microsoft.AspNetCore.Mvc;
using SyncthingWeb.Areas.Folders.Models;
using SyncthingWeb.Areas.Folders.Services;
using SyncthingWeb.Caching;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SyncthingWeb.Areas.Folders.ViewComponents
{
    public class ImageItemPreviewViewComponent : ViewComponent
    {
        private readonly ICache _cache;
        private readonly ISyncthingFileFetcher _fileFetcher;

        public ImageItemPreviewViewComponent(ISyncthingFileFetcher fileFetcher, ICache cache)
        {
            _fileFetcher = fileFetcher;
            _cache = cache;
        }

        public async Task<IViewComponentResult> InvokeAsync(PreviewableFileEntryContext context)
        {
            var imageDataUrl = await PreviewImageDataForAsync(context);

            return View(new ImageItemPreviewViewComponentViewModel { Context = context, ImageData = imageDataUrl});
        }

        private async  Task<string> PreviewImageDataForAsync(FileEntryContext context)
        {
            var cacheKey = $"{context.FolderId}-{context.Path}-{context.Name}";
            return await _cache.GetAsync(cacheKey, async cacheContext =>
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
                using (var image = new Bitmap(Image.FromStream(file)))
                {
                    const int size = 90;
                    int width, height;
                    if (image.Width > image.Height)
                    {
                        width = size;
                        height = Convert.ToInt32(image.Height * size / (double)image.Width);
                    }
                    else
                    {
                        width = Convert.ToInt32(image.Width * size / (double)image.Height);
                        height = size;
                    }

                    var resized = new Bitmap(width, height);
                    using (var graphics = Graphics.FromImage(resized))
                    {
                        graphics.CompositingQuality = CompositingQuality.HighSpeed;
                        graphics.InterpolationMode = InterpolationMode.Bilinear;
                        graphics.CompositingMode = CompositingMode.SourceCopy;
                        graphics.DrawImage(image, 0, 0, 90, 90);

                        var qualityParamId = Encoder.Quality;
                        var encoderParameters =
                            new EncoderParameters(1) {Param = {[0] = new EncoderParameter(qualityParamId, 75)}};
                        var codec = ImageCodecInfo.GetImageDecoders()
                            .FirstOrDefault(cdc => cdc.FormatID == ImageFormat.Jpeg.Guid);
                        resized.Save(ms, codec, encoderParameters);
                    }
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