using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SyncthingWeb.Areas.Share.Helpers;
using SyncthingWeb.Areas.Share.Permissions;
using SyncthingWeb.Areas.Share.Providers.Implementation;
using SyncthingWeb.Commands.Implementation.SharedEntries;

namespace SyncthingWeb.Areas.Share.Controllers
{
    [Area("Share")]
    public class PasswordProtectedController : ShareControllerBase
    {
        public PasswordProtectedController(ILogger<ShareControllerBase> logger) : base(logger)
        {
        }

        public override async Task<ActionResult> Configure(string id, string path)
        {
            if (!await this.Authorizer.AuthorizeAsync(SharePermissions.Allow)) return new UnauthorizedResult();
            if (!await this.HasAccess(id)) return new UnauthorizedResult();

            var vm = new ConfigureViewModel
            {
                Id = id,
                Path = path
            };

            return this.View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Configure(ConfigureViewModel vm)
        {
            if (!await this.Authorizer.AuthorizeAsync(SharePermissions.Allow)) return new UnauthorizedResult();

            if (!this.ModelState.IsValid) return this.View(vm);

            if (!await this.HasAccess(vm.Id)) return new UnauthorizedResult();

            var userClaims = this.User;
            var usrId = userClaims != null ? this.UserManager.GetUserId(userClaims) : null;
            var shareId = await
                this.CommandFactory.Create<WriteOrOverrideSharedEntryCommand>()
                    .Setup(vm.Id, usrId, vm.Path, PasswordProtectedShareProvider.GlobalId, FastHash(vm.Password)).GetAsync();

            return this.RedirectToAction("ShareSummary", new { id = shareId });
        }

        public override async Task<ActionResult> ShareSummary(int id)
        {
            if (!await this.Authorizer.AuthorizeAsync(SharePermissions.Allow)) return new UnauthorizedResult();

            var sh = await this.CommandFactory.Create<GetSharedEntryCommand>().Setup(id).GetAsync();
            if (sh == null) return this.NotFound();
            if (!await this.HasAccess(sh.Folder.FolderId)) return new UnauthorizedResult();

            this.ViewBag.SharedEntry = sh;
            return this.View(id);
        }

        [AllowAnonymous]
        public override async Task<ActionResult> Index(int id)
        {
            var sh = await this.CommandFactory.Create<GetSharedEntryCommand>().Setup(id).GetAsync();
            if (sh == null) return this.NotFound();
            if (sh.Provider != PasswordProtectedShareProvider.GlobalId) return this.NotFound();

            if (!await this.EnsureNonRemoved(sh)) return this.NotFound();

            var vm = new UnlockViewModel
            {
                Id = id
            };

            return this.View(vm);
        }

        [AllowAnonymous, HttpPost]
        public async Task<ActionResult> Index(UnlockViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                this.ModelState.AddModelError("Password", "Password is invalid.");
                return this.View(vm);
            }

            var sh = await this.CommandFactory.Create<GetSharedEntryCommand>().Setup(vm.Id).GetAsync();
            if (sh == null) return this.NotFound();
            if (sh.Provider != PasswordProtectedShareProvider.GlobalId) return this.NotFound();

            if (!await this.EnsureNonRemoved(sh)) return this.NotFound();

            var currentHash = this.FastHash(vm.Password);
            if (currentHash != sh.Data)
            {
                this.ModelState.AddModelError("Password", "Password is invalid.");
                return this.View(vm);
            }


            return await this.GetFileResultForLink(sh);
        }

        private string FastHash(string password)
        {
            var crypto = SHA1.Create();
            var resultBytes =
                crypto.ComputeHash(Encoding.UTF8.GetBytes(password + "{8B8263C9-038F-4DBF-B57E-E7521CC19367}" /* salt */));

            return Convert.ToBase64String(resultBytes);
        }

        public class UnlockViewModel
        {
            [Required]
            public int Id { get; set; }

            [Required]
            public string Password { get; set; }
        }

        public class ConfigureViewModel
        {
            [Required]
            public string Id { get; set; }

            [Required]
            public string Path { get; set; }

            [Required]
            public string Password { get; set; }
        }
    }
}