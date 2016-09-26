using System;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace SyncthingWeb.Commands
{
    internal class DefaultTraManager : ITranManager
    {
        private readonly DbContext context;

        public DefaultTraManager(DbContext context)
        {
            this.context = context;
        }

        public ITransaction Begin()
        {
            return new ConcreteTransaction(this.context.Database.BeginTransaction());
        }

        public ITransaction Begin(IsolationLevel isoLevel)
        {
            return new ConcreteTransaction(this.context.Database.BeginTransaction(isoLevel));
        }
    }

    internal class ConcreteTransaction : ITransaction
    {
        private readonly IDbContextTransaction transaction;

        public ConcreteTransaction(IDbContextTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            this.transaction = transaction;
        }

        public void Dispose()
        {
            this.transaction.Dispose();
        }

        public void Commit()
        {
            this.transaction.Commit();
        }

        public void Rollback()
        {
            this.transaction.Rollback();
        }
    }
}