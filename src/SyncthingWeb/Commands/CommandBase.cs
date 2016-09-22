using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.EntityFrameworkCore;
using SyncthingWeb.Data;
using SyncthingWebUI.Helpers;

namespace SyncthingWeb.Commands
{
    public abstract class CommandBase
    {
        public ILifetimeScope Resolver { get; set; }
        public ICommandFactory CommandFactory { get; set; }

        public ApplicationDbContext Context { get; set; }
    }

    public abstract class QueryCommand<T> : CommandBase
    {
        public abstract Task<IEnumerable<T>> ExecuteAsync();
    }

    public abstract class PagedQueryCommand<T, TQuery> : QueryCommand<T>
        where TQuery : PagedQuery
    {
        public PagingResult AdditionalResult { get; private set;}

        public TQuery Query { get; protected set; }

        public override async Task<IEnumerable<T>> ExecuteAsync()
        {
            var queryable = await this.GetQueryable();

            var count = await queryable.CountAsync();
            this.AdditionalResult = new PagingResult(count);

            var page = this.Query.Page > 0 ? this.Query.Page : 1;
            var newQueryable = queryable.Skip((page-1)*10).Take(10);

            return await newQueryable.ToListAsync();
        }

        public async Task<PagingResult<T, TQuery>> AsPagingResultAsync()
        {
            var items = await this.ExecuteAsync();
            return new PagingResult<T, TQuery>(items, this.Query, this.AdditionalResult.Count, 10);
        }

        protected abstract Task<IOrderedQueryable<T>> GetQueryable();

        public class PagingResult
        {
            public PagingResult(int count)
            {
                Count = count;
            }

            public int Count { get; }
        }
    }

    public abstract class GetCommand<T> : CommandBase
    {
        public abstract Task<T> GetAsync();
    }

    public abstract class NonQueryCommand : CommandBase
    {
        public abstract Task ExecuteAsync();
    }
}