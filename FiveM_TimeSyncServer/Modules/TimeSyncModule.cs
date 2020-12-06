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
            lastServerTime = LoadCurrentTime();
            lastRealTime = DateTime.Now;

            script.AddTick(AutosaveTime);
            script.AddTick(NetworkResync);
            script.AddTick(PeriodicConsolePrint);

            script.SetExport("GetTimeIsPaused", new Func<bool>(ExportGetTimeIsPaused));
            script.SetExport("SetTimeIsPaused", new Action<bool>(ExportSetTimeIsPaused));

            script.SetExport("GetCurrentDateTicks", new Func<long>(ExportGetCurrentDateTicks));
            script.SetExport("SetCurrentDateTicks", new Action<long>(ExportSetCurrentDateTicks));
        }

        #region ACCESSORS

        public DateTime CurrentDate
        {
            get
            {
                return lastServerTime.AddMilliseconds(timeElapsed);
            }
        }

        public bool Paused
        {
            get
            {
                return timePaused;
            }
            private set
            {
                if (value == true)
                {
                    lastServerTime = CurrentDate;
                }
                else
                {
                    lastRealTime = DateTime.Now;
                }
                timePaused = value;
            }
        }

        private double timeElapsed
        {
            get
            {
                if (Paused)
                {
                    return 0;
                }
                else
                {
                    return DateTime.Now.Subtract(lastRealTime).TotalMilliseconds * timeRate;
                }
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
        private bool timePaused;
        private DateTime lastServerTime;
        private DateTime lastRealTime;

        #endregion
        #region BASE EVENTS

        protected override void OnModuleInitialized()
        {
            // Print more informations
            verbose = API.GetConvarInt("timesync_network_verbose", 0) == 1;

            // Peridically print current time
            printEnabled = API.GetConvarInt("timesync_console_print_time", 1) == 1;

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

            script.Log($@"SERVER CURRENT TIME: {((Paused) ? "[PAUSED]" : "")}{CurrentDate.ToString(printFormat)}");
        }

        #endregion
        #region MODULE METHODS

        private void SetTimePaused(bool isPaused)
        {
            Paused = isPaused;
            Server.TriggerClientEvent("TimeSync.SetTimeIsPaused", Paused, CurrentDate.Ticks);
        }

        private void UpdatePlayerDateTime(Player player = null)
        {
            if (player != null)
            {
                Server.TriggerClientEvent(player, "TimeSync.UpdateDateTime", timeRate, Paused, CurrentDate.Ticks);
                if (verbose) script.Log($"FiveM TimeSync syncing player ${player.Name} time!");
            }
            else
            {
                if (API.GetNumPlayerIndices() > 0)
                {
                    Server.TriggerClientEvent("TimeSync.UpdateDateTime", timeRate, Paused, CurrentDate.Ticks);
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

        #endregion
        #region MODULE EXPORTS

        private bool ExportGetTimeIsPaused()
        {
            return Paused;
        }

        private void ExportSetTimeIsPaused(bool isPaused)
        {
            SetTimePaused(isPaused);
            script.Log($"Server time has been {((isPaused) ? "Paused" : "Unpaused")} at {CurrentDate.ToString(printFormat)} from an Export call.");
        }

        private long ExportGetCurrentDateTicks()
        {
            return CurrentDate.Ticks;
        }

        private void ExportSetCurrentDateTicks(long ticks)
        {
            lastServerTime = new DateTime(ticks);
            UpdatePlayerDateTime();
            script.Log($"Server time has been set to {CurrentDate.ToString(printFormat)} from an Export call.");
        }

        #endregion
    }
}
