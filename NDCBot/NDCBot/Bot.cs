using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using System.IO;
using System.Net;
using System;
using Newtonsoft.Json;
using System.Threading;
using System.Linq;

namespace NDCBot
{
    class Bot
    {
        private DiscordClient _client;
        private string token;
        public Bot()
        {
            Console.WriteLine("Starting bot...");
            try
            {
                token = File.ReadAllLines(Environment.CurrentDirectory + @"\t.token", Encoding.UTF8)[0];
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Failed to find token.txt! Automatically made one. Press any key to exit...");
                File.Create(Environment.CurrentDirectory + @"\t.token");
                Console.ReadKey();
                Environment.Exit(1);
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("token.txt file is empty! Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(1);
            }
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
                        OsuPlayer Player = JsonConvert.DeserializeObject<OsuPlayer[]>(responseContent)[0];
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
            _client.GetService<CommandService>().CreateCommand("kick")
                .Description("Kicks a user.")
                .Parameter("user", ParameterType.Required)
                .Do(async e =>
                {
                    if (e.User.ServerPermissions.KickMembers)
                    {
                        User user = null;
                        //try
                        //{
                            user = e.Server.FindUsers(e.GetArg("user")).First();
                        //}

                        if (user != null)
                        {
                            Console.WriteLine(user);
                            await user.Kick();
                            await e.Channel.SendMessage($"{user.Name} was kicked from the server!");
                        }
                        if (user == null)
                        {
                            await e.Channel.SendMessage($"User {e.GetArg("user")} not found.");
                        }
                    }
                });
            _client.GetService<CommandService>().CreateCommand("shutdown")
                .Description("Shuts the bot down.")
                .Do(async e =>
                {
                    if (e.User.ServerPermissions.Administrator)
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
            _client.GetService<CommandService>().CreateCommand("link")
            .Description("Get one of the links related to this discord server.")
            .Parameter("link", ParameterType.Required)
            .Do(async e =>
            {
                switch (e.GetArg("link").ToLower())
                {
                    case "git":
                    case "github":
                        await e.User.SendMessage("https://github.com/nitsoftdeveloperscommunity/");
                        break;
                    default:
                        await e.Channel.SendMessage("Unknown link.");
                        break;
                }
            });
            _client.ExecuteAndWait(async () =>
            {
                await _client.Connect(token, TokenType.Bot);
                _client.SetStatus(UserStatus.DoNotDisturb);
                _client.SetGame("https://discord.gg/VnwqcHC");
                Console.WriteLine("Bot Online");
            });
        }
    }
}
