using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SyncthingWeb.Models
{
    public class RolePermission
    {
        public virtual int Id { get; set; }

        public virtual string RoleId { get; set; }
        public virtual IdentityUserRole<string> Role { get; set; }

        [StringLength(300)]
        public virtual string Name { get; set; }
    }
}