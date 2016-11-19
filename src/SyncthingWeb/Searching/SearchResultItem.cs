namespace SyncthingWeb.Searching
{
    public class SearchResultItem
    {
        public SearchResultItem(string title, string smallTitle, string iconClass, string boxClass, string url)
        {
            Title = title;
            SmallTitle = smallTitle;
            IconClass = iconClass;
            BoxClass = boxClass;
            Url = url;
        }

        public string Title { get; }
        public string SmallTitle { get; }
        public string IconClass { get; }
        public string BoxClass { get; }

        public string Url { get; }
    }
}