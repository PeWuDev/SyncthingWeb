using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SyncthingWeb.Models
{
    public class Folder
    {
        public Folder()
        {
            this.Allowed = new HashSet<AllowedFolder>();
            this.Shared = new HashSet<SharedEntry>();
        }

        public virtual int Id { get; set; }

        [StringLength(32767)]
        public virtual string FolderId { get; set; }

        public virtual HashSet<AllowedFolder> Allowed { get; set; }
        public virtual HashSet<SharedEntry> Shared { get; set; }
    }
}