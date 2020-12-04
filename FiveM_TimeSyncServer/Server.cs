using VinaFrameworkServer.Core;
using FiveM_TimeSyncServer.Modules;

namespace FiveM_TimeSyncServer
{
    public class Server : BaseServer
    {
        public Server()
        {
            AddModule(typeof(TimeSyncModule));
        }
    }
}
