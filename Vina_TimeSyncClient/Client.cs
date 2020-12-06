using VinaFrameworkClient.Core;
using Vina_TimeSyncClient.Modules;

namespace Vina_TimeSyncClient
{
    public class Client : BaseClient
    {
        public Client()
        {
            UseGarbageCollector = true;

            AddModule(typeof(TimeSyncModule));
        }
    }
}
