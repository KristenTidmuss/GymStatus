# GymStatus
Discord bot to check status of TheGymGroup gyms and update it to display as a channel on a discord server
![demo image](https://i.imgur.com/3pUTfAz.png)

If you wish to use this personally you will need to update the channel ID codes in the discordBot() method. gCap is the capacity and gUp is the update time
```c#
  var gCap = await client.GetChannelAsync(931969047338319872) as SocketVoiceChannel;
  var gUp = await client.GetChannelAsync(931969363135836190) as SocketVoiceChannel;
```
You run the app using "dotnet run BranchID DiscordToken" or when published "dotnet GymStatus.dll BranchID DiscordToken"

Branch ID can be found by inspecting elements on the dropdown here: https://www.thegymgroup.com/gym-busyness/

A Discord bot can be created here: https://discord.com/developers/applications/
