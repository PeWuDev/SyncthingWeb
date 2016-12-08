using System;
using SyncthingWeb.Areas.Folders.Models;

namespace SyncthingWeb.Areas.Folders.Preview
{
    public abstract class PreviewProvider
    {
        public abstract Guid Id { get; }

        public abstract string ItemPreviewComponent { get; }
        public abstract string Title { get; }

        public abstract bool CanPreview(FileEntryContext entry);

        protected bool Equals(PreviewProvider other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PreviewProvider) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
