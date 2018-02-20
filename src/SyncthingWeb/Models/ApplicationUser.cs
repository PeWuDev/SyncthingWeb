using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SyncthingWeb.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            this.Allowed = new HashSet<AllowedFolder>();
            this.SharedEntried = new HashSet<SharedEntry>();
            this.IsEnabled = true;
        }

        public virtual HashSet<AllowedFolder> Allowed { get; set; }
        public virtual HashSet<SharedEntry> SharedEntried { get; set; }

        public virtual bool IsEnabled { get; set; }
    }
}
