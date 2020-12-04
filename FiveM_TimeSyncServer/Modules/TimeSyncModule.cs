using System;
using System.IO;
using System.Threading.Tasks;

using CitizenFX.Core;
using CitizenFX.Core.Native;

using VinaFrameworkServer.Core;

namespace FiveM_TimeSyncServer.Modules
{
    public class TimeSyncModule : Module
    {
        public TimeSyncModule(Server server) : base(server)
        {
            endingDate = LoadCurrentTime();
            lastServerTime = DateTime.Now;

            script.AddTick(NetworkResync);
            script.AddTick(AutosaveTime);
            script.AddTick(PeriodicConsolePrint);

            script.SetExport("CurrentDateTicks", new Func<long>(ExportCurrentDateTicks));
        }

        #region ACCESSORS

        public DateTime RealDate
        {
            get
            {
                return DateTime.Now;
            }
        }

        public TimeSpan RealTime
        {
            get
            {
                return RealDate.TimeOfDay;
            }
        }

        public DateTime CurrentDate
        {
            get
            {
                return endingDate.AddMilliseconds(timeElapsed);
            }
        }

        public TimeSpan CurrentTime
        {
            get
            {
                return CurrentDate.TimeOfDay;
            }
        }

        private double timeElapsed
        {
            get
            {
                return DateTime.Now.Subtract(lastServerTime).TotalMilliseconds * timeRate;
            }
        }

        #endregion
        #region VARIABLES

        private bool verbose;
        private bool printEnabled;
        private string printFormat;
        private int printDelay;
        private int clientUpdateDelay;
        private int timeRate;
        private DateTime endingDate;
        private DateTime lastServerTime;

        #endregion
        #region BASE EVENTS

        protected override void OnModuleInitialized()
        {
            // Print more informations
            verbose = API.GetConvarInt("timesync_network_verbose", 0) == 0;

            // Peridically print current time
            printEnabled = API.GetConvarInt("timesync_console_print_time", 0) == 0;

            // Console Print Time Format
            printFormat = API.GetConvar("timesync_console_print_format", "MMMM d yyyy, HH:mm:ss tt");

            // Console Print Time Delay
            printDelay = API.GetConvarInt("timesync_console_print_delay", 1000 * 60 * 5);

            // Sync players every 10 secs
            clientUpdateDelay = API.GetConvarInt("timesync_update_delay", 60000);

            // 10 x realtime
            timeRate = API.GetConvarInt("timesync_timerate", 1);

            Debug.WriteLine($@"
=====================================
FIVEM TIME SYNC SETTINGS:
=====================================
 timesync_console_print              = {printEnabled}
 timesync_console_print_format       = {printFormat}
 timesync_console_print_delay (ms)   = {printDelay}
 timesync_update_delay (ms)          = {clientUpdateDelay}
 timesync_timerate (1sec * timerate) = {timeRate}
 Current Date                        = {CurrentDate.ToString(printFormat)}
=====================================");
        }

        protected override void OnPlayerConnecting(Player player)
        {

        }

        protected override void OnPlayerDropped(Player player, string reason)
        {

        }

        protected override void OnPlayerClientInitialized(Player player)
        {
            UpdatePlayerDateTime(player);
        }

        #endregion
        #region MODULE TICKS

        private async Task NetworkResync()
        {
            await Server.Delay(clientUpdateDelay);

            UpdatePlayerDateTime();
        }

        private async Task AutosaveTime()
        {
            await Server.Delay(30000);

            await SaveCurrentTime();
        }

        private async Task PeriodicConsolePrint()
        {
            await Server.Delay(printDelay);

            if (!printEnabled) return;

            script.Log($@"SERVER CURRENT TIME: {CurrentDate.ToString(printFormat)}");
        }

        #endregion
        #region MODULE METHODS

        private void UpdatePlayerDateTime(Player player = null)
        {
            if (player != null)
            {
                Server.TriggerClientEvent(player, "TimeSync.UpdateDateTime", timeRate, CurrentDate.Ticks);
                if (verbose) script.Log($"FiveM TimeSync syncing player ${player.Name} time!");
            }
            else
            {
                if (API.GetNumPlayerIndices() > 0)
                {
                    Server.TriggerClientEvent("TimeSync.UpdateDateTime", timeRate, CurrentDate.Ticks);
                    if (verbose) script.Log($"FiveM TimeSync syncing all players time!");
                }
                else if (verbose) script.Log($"FiveM TimeSync no online players, skipping syncing...");
            }
        }

        private DateTime LoadCurrentTime()
        {
            try
            {
                string fileData = File.ReadAllText("server_time.txt");
                script.Log("Loaded time from server_time.txt");
                return DateTime.Parse(fileData);
            }
            catch (Exception exception)
            {
                script.Log("Could not get time from server_time.txt, starting with real time.");
            }

            return DateTime.Now;
        }

        private async Task SaveCurrentTime()
        {
            try
            {
                using (StreamWriter writer = File.CreateText("server_time.txt"))
                {
                    await writer.WriteAsync(CurrentDate.ToString());
                    writer.Close();
                }
            }
            catch (Exception exception)
            {
                script.LogError(exception);
            }
        }

        private long ExportCurrentDateTicks()
        {
            return CurrentDate.Ticks;
        }

        #endregion
    }
}
