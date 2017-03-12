using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Configuration;
using System.IO;

namespace NDCBot
{
    class Program
    {
        static void Main(string[] args) => new Program().Start();
        private DiscordClient _client;
        public static string strPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        private string token = File.ReadAllLines(strPath.Substring(6) + "\\token.txt", Encoding.UTF8)[0];
        public void Start()
        {
            _client = new DiscordClient();

            _client.MessageReceived += async (s, e) =>
            {
                if (!e.Message.IsAuthor)
                    await e.Channel.SendMessage(e.Message.Text);
            };
            _client.ExecuteAndWait(async () => {
                await _client.Connect(token, TokenType.Bot);
            });
        }
    }
}

