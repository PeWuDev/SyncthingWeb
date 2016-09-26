using System;
using System.Data;

namespace SyncthingWeb.Commands
{
    public interface ITranManager
    {
        ITransaction Begin();
        ITransaction Begin(IsolationLevel isoLevel);
    }

    public interface ITransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}
