using System.Collections.Generic;
using System.Threading.Tasks;

namespace SyncthingWeb.Searching
{
    public interface ISearchProvider
    {
        Task SearchAsync(string term, string user, ICollection<SearchResultItem> collection);
    }
}
