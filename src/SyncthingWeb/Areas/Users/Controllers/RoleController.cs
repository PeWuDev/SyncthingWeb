using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using SyncthingWeb.Areas.Users.Models;
using SyncthingWeb.Commands.Implementation.Users;
using SyncthingWeb.Helpers;
using SyncthingWeb.Permissions;
using SyncthingWebUI.Areas.Users.Permissions;

namespace SyncthingWeb.Areas.Users.Controllers
{
    [Authorize, Area("Users")]
    public class RoleController : ExtendedController
    {
        private readonly IPermissionResolver permResolver;

        public RoleController(IPermissionResolver permResolver)
        {
            this.permResolver = permResolver;
        }

        public async Task<ActionResult> Index(RoleQuery query)
        {
            if (!await this.Authorizer.AuthorizeAsync(RolePermissions.View)) this.Unauthorized();

            var collection = await this.CommandFactory.Create<PagedRolesCommand>().Setup(query).AsPagingResultAsync();

            return this.View(collection);
        }

        public async Task<ActionResult> Edit(string id = null)
        {
            if (!await this.Authorizer.AuthorizeAsync(RolePermissions.Manage)) return this.Unauthorized();

            if (id != null && !(await this.CommandFactory.Create<RoleExistsCommand>().SetupById(id).GetAsync()))
            {
                return this.NotFound();
            }

            var entity = id == null
                ? new IdentityRole()
                : await this.CommandFactory.Create<GetRoleCommand>().Setup(id).GetAsync();

            return this.View(entity);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName("Edit")]
        public async Task<ActionResult> EditPOST(string id)
        {
            if (!await this.Authorizer.AuthorizeAsync(RolePermissions.Manage)) return this.Unauthorized();

            var entity = id == null
                ? new IdentityRole()
                : await this.CommandFactory.Create<GetRoleCommand>().Setup(id).GetAsync();

            if (entity == null) entity = new IdentityRole();

            var orignalName = entity.Name;

            await this.TryUpdateModelAsync(entity);

            if (!this.ModelState.IsValid)
            {
                return this.View(entity);
            }

            if (id == null && await this.CommandFactory.Create<RoleExistsCommand>().SetupByName(entity.Name).GetAsync())
            {
                this.ModelState.AddModelError("Name", "Name is already used");
                return View(entity);
            }

            if (id != null && orignalName != entity.Name)
            {
                if (await this.CommandFactory.Create<RoleExistsCommand>().SetupByName(entity.Name).GetAsync())
                {
                    this.ModelState.AddModelError("Name", "Name is already used");
                    return View(entity);
                }
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(entity);
            }

            try
            {
                await this.CommandFactory.Create<UpdateRoleCommand>().Setup(entity).ExecuteAsync();

                this.Notifications.NotifySuccess("Role \"{0}\" saved successfully", entity.Name);
                return this.RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                this.Logger.ErrorException(ex, "Errpr while updating role.");
                this.Notifications.NotifyError("Unhandled exception while saving role.");

                return this.View(entity);
            }
        }

        public async Task<ActionResult> AssignPermissions(string id)
        {
            if (!await this.Authorizer.AuthorizeAsync(RolePermissions.Manage)) this.Unauthorized();

            if (!await this.CommandFactory.Create<RoleExistsCommand>().SetupById(id).GetAsync())
                return this.NotFound();

            var role = await this.CommandFactory.Create<GetRoleCommand>().Setup(id).GetAsync();
            var allPerms = this.permResolver.All as IList<Permission> ?? this.permResolver.All.ToList();
            var assignedNames =
                await this.CommandFactory.Create<QueryRolesPermissionsNamesCommand>().Setup(id).ExecuteAsync();
            var assigned = allPerms.Where(p => assignedNames.Contains(p.Name));

            return this.View(new AssignPermissionsToRoleViewModel(role, new HashSet<Permission>(assigned), allPerms));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignPermissions(AssignPermissionsToRoleViewModel model)
        {
            if (!await this.Authorizer.AuthorizeAsync(RolePermissions.Manage)) this.Unauthorized();

            if (!await this.CommandFactory.Create<RoleExistsCommand>().SetupById(model.RoleId).GetAsync())
                return this.NotFound();

            try
            {
                using (this.BeginTransaction())
                {
                    await this.permResolver.SetForRole(model.RoleId,
                        model.Perms.Where(p => p.Attached).Select(p => p.Name));
                }

                this.Notifications.NotifySuccess("Permissions for \"{0}\" saved successfully.", model.RoleName);
                return this.RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                this.Logger.ErrorException(ex, "Error while saving role permisisons.");
                this.Notifications.NotifyError("Unhandled exception while saving permissions for role. Try again later.");
                return this.View(model);
            }
        }


        public async Task<ActionResult> Assign(string id)
        {
            if (!await this.Authorizer.AuthorizeAsync(RolePermissions.Manage)) this.Unauthorized();

            var userIds = await this.CommandFactory.Create<QueryUsersInRolesCommand>().Setup(id).ExecuteAsync();

            this.ViewBag.RoleId = id;
            return this.View(userIds);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Assign(string roleId, string userId)
        {
            if (!await this.Authorizer.AuthorizeAsync(RolePermissions.Manage)) this.Unauthorized();

            if (!await this.CommandFactory.Create<RoleExistsCommand>().SetupById(roleId).GetAsync())
            {
                return this.NotFound();
            }

            await this.CommandFactory.Create<AssignRoleCommand>().Setup(roleId, userId).ExecuteAsync();

            return this.Json(new { error = false });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Deassign(string roleId, string userId)
        {
            if (!await this.Authorizer.AuthorizeAsync(RolePermissions.Manage)) this.Unauthorized();

            if (!await this.CommandFactory.Create<RoleExistsCommand>().SetupById(roleId).GetAsync())
            {
                return this.NotFound();
            }

            await this.CommandFactory.Create<DeassignRoleCommand>().Setup(roleId, userId).ExecuteAsync();

            return this.Json(new {error = false});
        }

        [HttpGet]
        public async Task<ActionResult> ToAssign(string roleId, string q)
        {
            if (string.IsNullOrWhiteSpace(roleId)) return this.NotFound();
            if (!await this.Authorizer.AuthorizeAsync(RolePermissions.Manage)) return this.Json(new object[0]);

            var users =
                await
                this.CommandFactory.Create<PageUsersCommand>()
                    .Setup(new GetUsersQuery { ExcludedRoles = new string[1] { roleId }, Name = q })
                    .ExecuteAsync();

            return this.Json(users.Select(u => new {u.Id, Text = u.UserName}));
        }
    }
}