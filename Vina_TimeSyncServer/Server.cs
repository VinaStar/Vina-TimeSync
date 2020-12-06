using VinaFrameworkServer.Core;
using Vina_TimeSyncServer.Modules;

namespace Vina_TimeSyncServer
{
    public class Server : BaseServer
    {
        public Server()
        {
            AddModule(typeof(TimeSyncModule));
        }
    }
}
