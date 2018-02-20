using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SyncthingWeb.Permissions;

namespace SyncthingWeb.Areas.Users.Models
{
    public class AssignPermissionsToRoleViewModel
    {
        public AssignPermissionsToRoleViewModel()
        {
        }

        public AssignPermissionsToRoleViewModel(IdentityRole role, HashSet<Permission> assigned, IEnumerable<Permission> allPermissions)
        {
            this.RoleId = role.Id;
            this.RoleName = role.Name;

            this.Perms = new List<AssignPermissionViewModel>();
            foreach (var perm in allPermissions)
            {
                this.Perms.Add(new AssignPermissionViewModel(perm, assigned.Contains(perm)));
            }
        }

        public string RoleId { get; set; }
        public string RoleName { get; set; }

        public List<AssignPermissionViewModel> Perms { get; set; }
    }

    public class AssignPermissionViewModel
    {
        public AssignPermissionViewModel()
        {

        }

        public AssignPermissionViewModel(Permission permisison, bool attached)
        {
            this.Attached = attached;
            this.Name = permisison.Name;
            this.Description = permisison.Description;
            this.Group = permisison.Group ?? string.Empty;
            this.Title = permisison.Title;
        }

        public bool Attached { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }
        public string Title { get; set; }
    }
}