namespace SyncthingWeb.Models
{
    public class AllowedFolder
    {
        public int Id { get; set; }

        public virtual string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual int FolderId { get; set; }
        public virtual Folder Folder { get; set; }

        //public bool CanShare { get; set; }
    }
}