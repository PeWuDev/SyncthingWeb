using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SyncthingWeb.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SyncthingWeb.Commands.Implementation.Users
{
    public class GetUserCommand : GetCommand<ApplicationUser>
    {
        public string Id { get; private set; }

        private readonly List<string> toInclude = new List<string>();

        public ReadOnlyCollection<string> ToInclude => new ReadOnlyCollection<string>(this.toInclude);

        public GetUserCommand Setup(string id)
        {
            this.Id = id;
            return this;
        }


        public GetUserCommand Include(string path)
        {
            this.toInclude.Add(path);
            return this;
        }

        public override Task<ApplicationUser> GetAsync()
        {
            var q = this.Context.Users.AsQueryable();

            /* TODO q = this.toInclude.Aggregate(q, (current, inc) => current.Include(inc)); */

            return q.FirstOrDefaultAsync(u => u.Id == this.Id);
        }
    }
}