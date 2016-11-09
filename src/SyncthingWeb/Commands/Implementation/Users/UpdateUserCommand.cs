using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SyncthingWeb.Models;

namespace SyncthingWeb.Commands.Implementation.Users
{

    public class UpdateUserCommand :NonQueryCommand
    {
        public Action<ApplicationUser> UpdateAction { get; private set; }
        public string Id { get; private set; }

        public UpdateUserCommand Setup(string id)
        {
            this.Id = id;
            return this;
        }

        public UpdateUserCommand Setup(Action<ApplicationUser> action)
        {
            this.UpdateAction = action;
            return this;
        }

        public override async Task ExecuteAsync()
        {
            var usr = await this.Context.Users.SingleAsync(u => u.Id == this.Id);
            this.UpdateAction?.Invoke(usr);
            await this.Context.SaveChangesAsync();
        }
    }
}