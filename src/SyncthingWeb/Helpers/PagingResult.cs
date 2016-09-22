namespace SyncthingWebUI.Helpers
{
    using System.Collections.Generic;

    public class PagingResult<TElement, TQuery>
        where TQuery : PagedQuery
    {
        public PagingResult(IEnumerable<TElement> items, TQuery query, int count, int pageSize)
        {
            Items = items;
            Query = query;
            Count = count;
            PageSize = pageSize;
        }

        public IEnumerable<TElement> Items { get; private set; }
        public TQuery Query { get; private set; }
        public int Count { get; private set; }
        public int PageSize { get; private set; }
    }
}