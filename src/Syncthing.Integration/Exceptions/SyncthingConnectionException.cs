using System;

namespace Syncthing.Integration.Exceptions
{
    [Serializable]
    public class SyncthingConnectionException : Exception
    {
        public SyncthingConnectionException()
        {
        }

        public SyncthingConnectionException(string message) : base(message)
        {
        }

        public SyncthingConnectionException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
