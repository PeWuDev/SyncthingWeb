using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SyncthingWeb.Searching
{
    public interface ISearchCollector
    {
        string Term { get; }
        string User { get; }
        void Add(SearchResultItem result);
    }

    public class SearchCollector : ISearchCollector
    {
        private readonly IList<SearchResultItem> items = new List<SearchResultItem>();

        public SearchCollector(string term, string user)
        {
            Term = term;
            User = user;
        }

        public string Term { get; }
        public string User { get; }

        public IReadOnlyCollection<SearchResultItem> Items => new ReadOnlyCollection<SearchResultItem>(this.items);

        public void Add(SearchResultItem result)
        {
            this.items.Add(result);
        }
    }
}
