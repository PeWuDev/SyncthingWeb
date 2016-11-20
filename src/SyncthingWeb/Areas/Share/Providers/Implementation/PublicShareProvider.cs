using System;

namespace SyncthingWeb.Areas.Share.Providers.Implementation
{
    public class PublicShareProvider : ShareProviderBase
    {
        public static Guid GlobalId = Guid.Parse("{BF0ABED9-E964-4016-8BB4-B0C0AF6C8809}");
        public override Guid Id { get; } = GlobalId;

        public override string Title { get; } = "Public";
        public override string Decription { get; } = "Create a public link that can access everyone";
        public override string IconCss { get; } = "fa-globe";

        public override string AreaOfController { get; } = "Share";
        public override string NameOfController { get; } = "Public";
    }
}