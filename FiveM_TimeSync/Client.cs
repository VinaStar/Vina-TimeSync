using VinaFrameworkClient.Core;
using FiveM_TimeSync.Modules;

namespace FiveM_TimeSync
{
    public class Client : BaseClient
    {
        public Client()
        {
            AddModule(typeof(TimeSyncModule));
        }
    }
}
