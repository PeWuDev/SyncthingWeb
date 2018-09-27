using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace SyncthingWeb.Data
{
#if DEBUG
    public class TemporaryDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        //public ApplicationDbContext Create()
        //{
        //    var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        //    builder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=pinchdb;Trusted_Connection=True;MultipleActiveResultSets=true");
        //    return new ApplicationDbContext(builder.Options);
        //}

        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            builder.UseSqlite("Filename=SyncthingWebDatabase.db");
            return new ApplicationDbContext(builder.Options);
        }
    }
#endif
}
