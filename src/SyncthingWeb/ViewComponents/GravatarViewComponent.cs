using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SyncthingWeb.ViewComponents
{
    public class GravatarViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string userEmail, int size, string additionalClass = null)
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                return Content(string.Empty);
            }

            return View(new GravatarViewComponentParam
            {
                AdditionalClass = additionalClass,
                Size = size,
                EmailHash = GetMd5Hash(MD5.Create(), userEmail)
            });
        }

        private static string GetMd5Hash(HashAlgorithm hashAlg, string input)
        {
            var data = hashAlg.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sBuilder = new StringBuilder();            
            foreach (var @byte in data)
            {
                sBuilder.Append(@byte.ToString("x2"));
            }
            
            return sBuilder.ToString();
        }
    }

    public class GravatarViewComponentParam
    {
        public string EmailHash { get; set; }
        public int Size { get; set; }

        public string AdditionalClass { get; set; }
    }
}
