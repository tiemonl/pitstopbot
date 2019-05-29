using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace PitStopBot.Commands {
    public class GeneralCommands : ModuleBase<SocketCommandContext> {
        CommandService commands;
        public GeneralCommands(CommandService _commands) {
            commands = _commands;
        }
        [Command("ping"), Summary("Test to see if bot works.")]
        public async Task Pong() {
            await ReplyAsync($"pong");
        }
        [Command("Help")]
        public async Task Help() {
            EmbedBuilder embedBuilder = new EmbedBuilder();

            foreach (CommandInfo command in commands.Commands) {
                // Get the command Summary attribute information
                string embedFieldText = command.Summary ?? "No description available\n";

                embedBuilder.AddField(command.Name, embedFieldText);
            }

            await ReplyAsync("Here's a list of commands and their description: ", false, embedBuilder.Build());
        }
    }
}