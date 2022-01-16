using Discord;
using Discord.Commands;
using Discord.Rest;
using Newtonsoft.Json.Linq;
using Discord.WebSocket;
//Branch for Nottingham Sherwood: 8a2d9772-268f-4d62-8291-f7bb28f10302
namespace GymStatus
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("dotnet GymStatus.dll BranchID DiscordToke");
                Console.WriteLine("Please enter Branch ID and Discord bot Token");
                Console.WriteLine("Branch ID can be found by inspecting elements on the dropdown here: https://www.thegymgroup.com/gym-busyness/");
                Console.WriteLine("A Discord bot can be created here: https://discord.com/developers/applications/");
                return;
            }
            discordBot(args[0],args[1]).GetAwaiter().GetResult();
        }
        static async Task discordBot(string branchid, string token)
        {
            var client = new DiscordSocketClient();
            //var token = "";
            client.Log += LogAsync;
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync(); 
            client.Ready += async () =>
            {
                while (true)
                {
                    try
                    {
                        var gymStats = await getGymStatus(branchid);
                        var gCap = await client.GetChannelAsync(931969047338319872) as SocketVoiceChannel;
                        var gUp = await client.GetChannelAsync(931969363135836190) as SocketVoiceChannel;
                        var emoji = gymStats[2] switch
                        {
                            "lower" => "🟢",
                            "middle" => "🟡",
                            "upper" => "🔴",
                            _ => ""
                        };
                        await gCap.ModifyAsync(prop => prop.Name = $"{emoji} Capacity: {gymStats[1].Split(" ")[0]}");
                        await gUp.ModifyAsync(prop => prop.Name = $"⏱️ Updated: {gymStats[3].Split(" ")[1]}");
                        Console.WriteLine($"Updated Status: {DateTime.UtcNow}");
                        Thread.Sleep(60000);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                
            };
            // Block this task until the program is closed.
            await Task.Delay(-1);
        }
        
        static async Task<string[]> getGymStatus(string branchid)
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync($"https://www.thegymgroup.com/BranchGymBusynessBlock/GetBusynessForBranch/?branchId={branchid}&configurationId=0749f44e-4aa9-495d-a37a-d84a36d4b999");
                var content = await response.Result.Content.ReadAsStringAsync();
                var jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(content);

                if (jsonResult != null)
                {
                    string[] returnVal = new string[4];
                    returnVal[0] = jsonResult.currentBranch.name;
                    returnVal[1] = jsonResult.currentBranch.capacity;
                    returnVal[2] = jsonResult.currentBranch.threshold;
                    returnVal[3] = jsonResult.currentBranch.lastUpdated;
                    return returnVal;
                }
                return null;
            }
        }
        
        private static Task LogAsync(LogMessage message)
        {
            if (message.Exception is CommandException cmdException)
            {
                Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}"
                                  + $" failed to execute in {cmdException.Context.Channel}.");
                Console.WriteLine(cmdException);
            }
            else 
                Console.WriteLine($"[General/{message.Severity}] {message}");

            return Task.CompletedTask;
        }
    }
}
