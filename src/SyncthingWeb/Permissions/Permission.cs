namespace SyncthingWebUI.Permissions
{
    using System.Collections.Generic;

    public class Permission
    {
        public Permission(string name, string title)
        {
            this.Name = name;
            Title = title;
            this.Implies = new HashSet<string>();
        }

        public string Name { get; }
        public string Title { get; }
        public string Description { get; set; }
        public string Group { get; set; }
        public HashSet<string> Implies { get; set; }

        protected bool Equals(Permission other)
        {
            return string.Equals(this.Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return this.Equals((Permission)obj);
        }

        public override int GetHashCode()
        {
            return this.Name?.GetHashCode() ?? 0;
        }
    }
}