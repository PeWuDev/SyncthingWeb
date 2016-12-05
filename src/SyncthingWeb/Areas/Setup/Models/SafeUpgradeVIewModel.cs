using System.ComponentModel.DataAnnotations;
using SyncthingWeb.Models;

namespace SyncthingWeb.Areas.Setup.Models
{
    public class SafeUpgradeVIewModel
    {
        public SafeUpgradeVIewModel()
        {
            
        }

        public SafeUpgradeVIewModel(GeneralSettings entity)
        {
            this.SyncthingEndpoint = entity.SyncthingEndpoint;
            this.SyncthingApiKey = entity.SyncthingApiKey;
        }

        [Required(ErrorMessage = "Endpoing address is required.")]
        public string SyncthingEndpoint { get; set; }

        [Required(ErrorMessage = "Endpoing API key is required.")]
        public string SyncthingApiKey { get; set; }
    }
}
