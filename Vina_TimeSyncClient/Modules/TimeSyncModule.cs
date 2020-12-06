using System;
using System.Threading.Tasks;

using CitizenFX.Core.Native;

using VinaFrameworkClient.Core;

namespace Vina_TimeSyncClient.Modules
{
    public class TimeSyncModule : Module
    {
        public TimeSyncModule(Client client) : base(client)
        {
            script.AddEvent("TimeSync.UpdateDateTime", new Action<int, bool, long>(OnUpdateDateTime));
            script.AddEvent("TimeSync.SetTimeIsPaused", new Action<bool, long>(OnSetTimeIsPaused));
        }

        #region ACCESSORS

        public DateTime CurrentDate
        {
            get
            {
                return lastServerTime.AddMilliseconds(timeElapsed);
            }
        }

        public TimeSpan CurrentTime
        {
            get
            {
                return CurrentDate.TimeOfDay;
            }
        }

        public bool Paused
        {
            get
            {
                return timePaused;
            }
            set
            {
                lastRealTime = DateTime.Now;
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

        private int timeRate;
        private bool timePaused;
        private DateTime lastRealTime;
        private DateTime lastServerTime;

        #endregion
        #region BASE EVENTS

        protected override void OnModuleInitialized()
        {
            API.SetMillisecondsPerGameMinute(0);
            script.AddTick(OverrideTime);
        }

        #endregion
        #region MODULE EVENTS

        private void OnUpdateDateTime(int newTimeRate, bool isPaused, long currentTicks)
        {
            timeRate = newTimeRate;
            lastRealTime = DateTime.Now;
            lastServerTime = new DateTime(currentTicks);
            Paused = isPaused;

            script.Log($"Received update from server [TimeRate: {timeRate}, Server Time: ${lastServerTime}]");
        }

        private void OnSetTimeIsPaused(bool isPaused, long currentTicks)
        {
            Paused = isPaused;
            if (Paused) lastServerTime = new DateTime(currentTicks);
            script.Log($"Server time was {((isPaused) ? "Paused" : "Unpaused")}");
        }

        #endregion
        #region MODULES TICKS

        private async Task OverrideTime()
        {
            while (true)
            {
                await Client.Delay(50);

                if (lastRealTime == null || lastServerTime == null) continue;

                API.NetworkOverrideClockTime(CurrentTime.Hours, CurrentTime.Minutes, CurrentTime.Seconds);
            }
        }

        #endregion
    }
}
