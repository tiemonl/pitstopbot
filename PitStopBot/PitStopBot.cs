using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace PitStopBot {
    public class PitStopBot {
        private readonly DiscordSocketClient client;
        public string CommandPrefix = "-";
        public CommandService commands;
        private readonly GetKey keyGetter;

        public PitStopBot() {
            client = new DiscordSocketClient(new DiscordSocketConfig {
                LogLevel = LogSeverity.Info
            });
            keyGetter = new GetKey();
            commands = new CommandService(new CommandServiceConfig {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async
            });
            client.MessageReceived += HandleCommandAsync;
            client.Log += Log;

        }

        public async Task MainAsync() {
            Console.WriteLine("Which bot to run: ");
            string key = Console.ReadLine();
            if (key.Equals("debug")) {
                CommandPrefix = "?";
            }
            string token = keyGetter.Get(key).Trim();

            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            await client.LoginAsync(TokenType.Bot, token, false);
            await client.StartAsync();

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg) {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private async Task HandleCommandAsync(SocketMessage arg) {
            // Bail out if it's a System Message.
            if (!(arg is SocketUserMessage msg))
                return;

            // We don't want the bot to respond to itself or other bots.
            if (msg.Author.Id == client.CurrentUser.Id || msg.Author.IsBot)
                return;

            // Create a number to track where the prefix ends and the command begins
            int pos = 0;
            // Replace the '!' with whatever character
            // you want to prefix your commands with.
            // Uncomment the second half if you also want
            // commands to be invoked by mentioning the bot instead.
            if (msg.HasStringPrefix(CommandPrefix, ref pos) /* || msg.HasMentionPrefix(_client.CurrentUser, ref pos) */) {
                // Create a Command Context.
                var context = new SocketCommandContext(client, msg);

                // Execute the command. (result does not indicate a return value, 
                // rather an object stating if the command executed successfully).
                var result = await commands.ExecuteAsync(context, pos, null);

                // Uncomment the following lines if you want the bot
                // to send a message if it failed.
                // This does not catch errors from commands with 'RunMode.Async',
                // subscribe a handler for '_commands.CommandExecuted' to see those.
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    await msg.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
    }
}
