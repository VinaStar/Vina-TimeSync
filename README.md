# FiveM TimeSync

### FEATURES
- Perfect Client & Server time synchronization
- Can configure resource with convars
- Can start server with a custom time rate
- Can pause time serverside
- Time Persistence when rebooting
- Standalone Resource
- Can use Exports methods in your own resources
   
---
  
**Client Performance**  
![Client Performance](https://i.imgur.com/M0Mvbia.png)  
  
**Server Performance**  
![Server Performance](https://i.imgur.com/Zpz8Gte.png)  
  
**Server Console Output**  
![Server Console Output](https://i.imgur.com/GSkoVZB.png)  
  
---
  
### DEPENDENCIES:
- [Vina Framework](https://github.com/VinaStar/Vina-Framework/releases)
  
---
   
### INSTRUCTIONS:
   
   **1)** Place "fivemtimesync" directory inside your server Resources directory.
   
   **2)** Add "ensure fivemtimesync" to your server config.
   
   **3)** Start your FiveM server.
   
   **4)** Will create a "server_time.txt" automatically at first launch, this is were time will be saved for persistence.
   
---
   
### SERVER EXPORTS:
   
You can get the server time in your own resource using this export function:  
- GetTimeIsPaused()  
**return:** bool  
**info:** Get if the server time is paused  
  
- SetTimeIsPaused(**bool** isPaused)  
**info:** Pause the server time  
  
- GetCurrentDateTicks()  
**return:** long  
**info:** Get the server time total ticks  
  
- SetCurrentDateTicks(**long** ticks)  
**info:** Set the server time from total ticks  
   
```csharp
// Get the server time
long ticks = Exports["fivemtimesync"].GetCurrentDateTicks();
DateTime currentDate = new DateTime(ticks);

// Set the server time
Exports["fivemtimesync"].SetCurrentDateTicks(DateTime.Now.Ticks);
```
   
---
   
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
   
   
