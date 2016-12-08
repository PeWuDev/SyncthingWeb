namespace SyncthingWeb.Models
{
    public class GeneralSettings
    {
        public GeneralSettings()
        {
            this.EnableRegistration = true;
        }

        public virtual int Id { get; set; }
        public virtual bool Initialzed { get; set; }

        public virtual string SyncthingEndpoint { get; set; }
        public virtual string SyncthingApiKey { get; set; }

        public virtual string AdminId { get; set; }
        public virtual ApplicationUser Admin { get; set; }

        public virtual bool EnableRegistration { get; set; }

        public bool NeedsUpgrade()
            => string.IsNullOrEmpty(this.SyncthingApiKey) || string.IsNullOrEmpty(this.SyncthingEndpoint);
    }
}