using System;
using System.Threading.Tasks;

using CitizenFX.Core.Native;

using VinaFrameworkClient.Core;

namespace FiveM_TimeSync.Modules
{
    public class TimeSyncModule : Module
    {
        public TimeSyncModule(Client client) : base(client)
        {
            script.AddEvent("TimeSync.UpdateDateTime", new Action<int, long>(OnUpdateDateTime));
            script.AddTick(OverrideTime);
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

        private double timeElapsed
        {
            get
            {
                return DateTime.Now.Subtract(startingDate).TotalMilliseconds * timeRate;
            }
        }

        #endregion
        #region VARIABLES

        private int timeRate;
        private DateTime startingDate;
        private DateTime lastServerTime;

        #endregion
        #region MODULE EVENTS

        private void OnUpdateDateTime(int newTimeRate, long currentTicks)
        {
            timeRate = newTimeRate;
            startingDate = DateTime.Now;
            lastServerTime = new DateTime(currentTicks);

            script.Log($"Received update from server [TimeRate: {timeRate}, Server Time: ${lastServerTime}]");
        }

        #endregion
        #region MODULES TICKS

        private async Task OverrideTime()
        {
            while (true)
            {
                await Client.Delay(33);

                if (startingDate == null || lastServerTime == null) continue;

                API.NetworkOverrideClockTime(CurrentTime.Hours, CurrentTime.Minutes, CurrentTime.Seconds);
            }
        }

        #endregion
    }
}
