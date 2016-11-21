using System;

namespace SyncthingWeb.Areas.Share.Providers.Implementation
{
    public class PasswordProtectedShareProvider : ShareProviderBase
    {
        public static readonly Guid GlobalId = Guid.Parse("{8620E979-6938-46D7-AD67-FF5CB84F37A5}");
        public override Guid Id { get; } = GlobalId;
        public override string Title { get; } = "Password protected";

        public override string Decription { get; } =
            "The owner of link needs to provide a password in order to access a file.";

        public override string IconCss { get; } = "fa-lock";

        public override string AreaOfController { get; } = "Share";
        public override string NameOfController { get; } = "PasswordProtected";
    }
}