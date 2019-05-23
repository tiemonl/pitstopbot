using System.Threading.Tasks;
using Discord.Commands;

namespace PitStopBot.Commands {
	public class GeneralCommands : ModuleBase {
		public GeneralCommands() {
		}
		[Command("ping"), Summary("Test to see if bot works.")]
		public async Task Pong() {
			await ReplyAsync($"pong");
		}
	}
}
