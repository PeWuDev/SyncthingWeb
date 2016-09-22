using System;
using System.ComponentModel.DataAnnotations;
using SyncthingWeb.Helpers;
using SyncthingWebUI.Helpers;

namespace SyncthingWeb.Models
{
    public class SharedEntry
    {
        public virtual int Id { get; set; }

        [Required]
        public virtual int FolderId { get; set; }
        public virtual Folder Folder { get; set; }

        public virtual Guid Provider { get; set; }

        [Required]
        public virtual string OwnerId { get; set; }
        public virtual ApplicationUser Owner { get; set; }

        [Required]
        public virtual string Path { get; set; }

        public virtual DateTime ShareTime { get; set; }

        public virtual string  Data { get; set; }

        public string GetDirectory()
        {
            return PathHelpers.GetDirPathFromRelativePath(this.Path);
        }
    }
}