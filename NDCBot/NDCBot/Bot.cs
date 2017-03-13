using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using System.IO;
using System.Net;
using System;
using Newtonsoft.Json;
using System.Threading;

namespace NDCBot
{
    class Bot
    {
        private DiscordClient _client;
        public static string strPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        private string token = File.ReadAllLines(strPath.Substring(6) + "\\token.txt", Encoding.UTF8)[0];

        public Bot()
        {
            Console.WriteLine("Starting bot...");
            Run();
            Console.Clear();
            Console.WriteLine("Bot running...");
        }

        private void Run()
        {
            _client = new DiscordClient();
            _client.UsingCommands(x =>
            {
                x.PrefixChar = '>';
                x.HelpMode = HelpMode.Private;
            });

            _client.MessageReceived += async (s, e) =>
            {
                if (!e.Message.IsAuthor)
                {
                    if (e.Message.Channel.Id == 285639013929451521)
                    {
                        await e.Message.Delete();
                    }
                }
            };

            _client.UserJoined += async (s, e) =>
            {
                await e.Server.DefaultChannel.SendMessage($"Please welcome **{e.User.Mention}** to **{e.Server.Name}**!");
                await e.Server.GetChannel(285491947693408257).SendMessage($"User {e.User.Mention} joined.");
            };

            _client.GetService<CommandService>().CreateCommand("osu")
                .Alias(new string[] { "osuinfo", "osuplayer" })
                .Description("Gets the osu!user information of a player.")
                .Parameter("player", ParameterType.Required)
                .Do(async e =>
                {
                    byte[] data = Encoding.ASCII.GetBytes("k=b3c9ddc520647c11eb3eddd894d26a095fe32883&u=" + e.GetArg("player"));
                    WebRequest request = WebRequest.Create("http://osu.ppy.sh/api/get_user");
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = data.Length;
                    using (Stream stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    string responseContent = null;

                    using (WebResponse response = request.GetResponse())
                    {
                        using (Stream stream = response.GetResponseStream())
                        {
                            using (StreamReader sr = new StreamReader(stream))
                            {
                                responseContent = sr.ReadToEnd();
                            }
                        }
                        Player Player = JsonConvert.DeserializeObject<Player[]>(responseContent)[0];
                        await e.Channel.SendMessage($"{Player.country} Player information for: ``{Player.username}``{'\n'}Profile: http://osu.ppy.sh/u/{Player.user_id}{'\n'}Total Score: {Player.total_score.ToString()}");
                        await e.Channel.SendMessage($"Accuracy: {Player.accuracy} {'\n'}Level: {Player.level}{'\n'}PP: {Player.pp_raw}");
                    };
                });
            _client.GetService<CommandService>().CreateCommand("agree")
                .Description("Agrees to the rules.")
                .Do(async e =>
                {
                    await e.User.AddRoles(e.Server.GetRole(285639013602164737));
                });
            _client.GetService<CommandService>().CreateCommand("shutdown")
                .Description("Shuts the bot down.")
                .Do(async e =>
                {
                    if (e.User.Id == 199273626686455818)
                    {
                        await e.Server.DefaultChannel.SendMessage("Shutting down for repairs! :wrench: :wave:");
                        Thread.Sleep(1000);
                        await _client.Disconnect();
                        Environment.Exit(0);
                    }
                    else
                    {
                        await e.Channel.SendMessage("No thanks.");
                        await e.Server.GetUser(199273626686455818).SendMessage($"User **@{e.User.Name}#{e.User.Discriminator}** on server **{e.Server.Name}** tried to shutdown the bot. I don't think thats gonna happen.");
                    }
                });
            _client.ExecuteAndWait(async () =>
            {
                await _client.Connect(token, TokenType.Bot);
                _client.SetStatus(UserStatus.Idle);
                _client.SetGame("https://discord.gg/VnwqcHC");

                Console.WriteLine("Bot Online");
            });
        }
    }
}
