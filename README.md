# FiveM TimeSync v1.0

### FEATURES
- Perfect Client & Server time synchronization
- Configurable server settings using Convars
- Custom Timerate speed
- Time Persistence
- Standalone Resource
- Compatible with any resources
   
   
### INSTRUCTIONS:
   
   **1)** Place "fivemtimesync" directory inside your server Resources directory.
   
   **2)** Add "ensure fivemtimesync" to your server config.
   
   **3)** Start your FiveM server.
   
   **4)** Will create a "server_time.txt" automatically at first launch, this is were time will be saved for persistence.
   
### EXPORTS:
   
You can get the server time in your own resource using this export function:  
   *Exports["fivemtimesync"].CurrentDateTicks()*  
   **Will return the total Ticks**
   
   ```C#
   DateTime currentDate = new DateTime(Exports["fivemtimesync"].CurrentDateTicks());
   ```
   
   
### CONVARS:
   
You can change the settings using convar in your FiveM server config file:
   
- *set timesync_network_verbose 0*  
**Set to 1 to print event in the console**

- *set timesync_console_print_time 0*  
**Set to 1 to print the time periodically in the console**

- *set timesync_console_print_format "MMMM d yyyy, HH:mm:ss tt"*  
**Set the format to print the date/time**

- *set timesync_console_print_delay 60000*  
**Set the millisecond delay between the print of time in console**

- *set timesync_update_delay 60000*  
**Set the delay before server force resync with all players**

- *set timesync_timerate 1*  
**Set the timerate, 1 = 1 real second & 10 = 10 time faster than real time**
   
   
