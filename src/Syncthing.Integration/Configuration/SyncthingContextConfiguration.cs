namespace Syncthing.Integration.Configuration
{
    public class SyncthingContextConfiguration
    {
        public delegate void ErrorCallback(string message);

        internal ErrorCallback OnErrorCallback = _ => { };

        public SyncthingContextConfiguration SetErrorCallback(ErrorCallback onErrorCallback)
        {
            this.OnErrorCallback = onErrorCallback;
            return this;
        }


    }
}
