using System.Threading.Tasks;
using SyncthingWeb.Commands;
using SyncthingWeb.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SyncthingWebUI.Commands.Implementations.Users
{

    public class SetPermissionsForRoleCommand : NonQueryCommand
    {
        public string RoleId { get; private set; }
        public string[] Permisions { get; private set; }

        public SetPermissionsForRoleCommand Setup(string roleId, params string[] permissions)
        {
            this.RoleId = roleId;
            this.Permisions = permissions;
            return this;
        }

        public override  async Task ExecuteAsync()
        {

            var toDelete = await this.Context.RolePermissions.Where(rp => rp.RoleId == this.RoleId).ToListAsync();
            this.Context.RolePermissions.RemoveRange(toDelete);

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var pn in this.Permisions)
            {
                this.Context.RolePermissions.Add(new RolePermission
                {
                    RoleId = this.RoleId,
                    Name = pn
                });
            }

            await this.Context.SaveChangesAsync();
        }
    }
}