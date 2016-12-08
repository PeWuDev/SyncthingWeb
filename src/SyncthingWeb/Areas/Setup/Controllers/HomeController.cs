using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SyncthingWeb.Areas.Setup.Models;
using SyncthingWeb.Commands.Implementation.Settings;
using SyncthingWeb.Helpers;
using SyncthingWeb.Models;

namespace SyncthingWeb.Areas.Setup.Controllers
{
    [Area("Setup")]
    public class HomeController : ExtendedController
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<HomeController> logger;

        public HomeController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<HomeController> logger)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.logger = logger;
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return this.View(new SetupViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> Index(SetupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            IdentityResult result;

            this.logger.LogInformation(SetupLoggingEvents.ConfigureApp, "Creating super admin user \"{0}\"", model.Email);
            result = await this.userManager.CreateAsync(user, model.Password);


            if (!result.Succeeded)
            {
                //TODO this.Logger.Error(
                //    "Error while registering user \"{0}\": {1}",
                //    model.Email,
                //    string.Join(", ", result.Errors));

            }

            if (result.Succeeded)
            {
                await
                    this.CommandFactory.Create<UpdateGeneralSettingsCommand>()
                        .Setup(
                            s =>
                            {
                                s.Initialzed = true;
                                s.SyncthingEndpoint = model.SyncthingEndpoint;
                                s.SyncthingApiKey = model.SyncthingApiKey;
                                s.AdminId = user.Id;
                                s.EnableRegistration = model.EnableRegistration;
                            })
                        .ExecuteAsync();

                await signInManager.SignInAsync(user, isPersistent: true);
                this.Notifications.NotifySuccess("Website was setup properly!");
                return RedirectToAction("Index", "Home", new { area = "" });
            }

            foreach (var err in result.Errors) this.Notifications.NotifyError(err.Description);
            return this.View(model);
        }

        [AllowAnonymous]
        public async Task<ActionResult> SafeUpgrade()
        {
            var entity = await this.CommandFactory.Create<GetCurrentGeneralSettingsCommand>().GetAsync();
            if (!entity.NeedsUpgrade())
            {
                return this.Unauthorized();
            }

            return this.View(new SafeUpgradeVIewModel(entity));
        }


        [ValidateAntiForgeryToken, HttpPost]
        public async Task<ActionResult> SafeUpgrade(SafeUpgradeVIewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.View(model);
            }

            await this.CommandFactory.Create<UpdateGeneralSettingsCommand>().Setup(gs =>
            {
                gs.SyncthingEndpoint = model.SyncthingEndpoint;
                gs.SyncthingApiKey = model.SyncthingApiKey;
            }).ExecuteAsync();

            this.Notifications.NotifySuccess("Settings saved successfully.");
            return this.RedirectToAction("Index", "Home", new {area = ""});
        }

        public async Task<ActionResult> Configuration()
        {
            var vm = await this.CommandFactory.Create<GetCurrentGeneralSettingsCommand>().GetAsync();
            return this.View(vm);
        }

        [ValidateAntiForgeryToken, HttpPost, ActionName("Configuration")]
        public async Task<ActionResult> ConfigurationPOST(GeneralSettings model)
        {
            await this.CommandFactory.Create<UpdateGeneralSettingsCommand>().Setup(gs =>
            {
                gs.SyncthingEndpoint = model.SyncthingEndpoint;
                gs.SyncthingApiKey = model.SyncthingApiKey;
                gs.EnableRegistration = model.EnableRegistration;
            }).ExecuteAsync();

            this.Notifications.NotifySuccess("Settings saved successfully.");
            return this.RedirectToAction("Configuration");
        }
    }
}