using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using PitStopBot.Utils;

namespace PitStopBot.Commands {
    [Group("giveaway"), Alias("g")]
    public class Giveaway : ModuleBase {
        public Dictionary<ulong, GiveawayInstance> instanceDictionary;
        public Giveaway() {
            instanceDictionary = new Dictionary<ulong, GiveawayInstance>();
        }

        [Command("randomdraw", RunMode = RunMode.Async), Alias("rand", "draw", "rd"), Summary("Users will react to the message provided by the bot and it will randomly pick a user to win.")]
        public async Task RandomDraw([Summary("seconds to wait before drawing the winner")] int seconds,
        [Summary("What the prize is for the winner."), Remainder] string prize) {
            if (instanceDictionary.TryGetValue(Context.Channel.Id, out GiveawayInstance newInstance)) {
                await ReplyAsync("Game already running. Please wait until it is over");
            } else {
                newInstance = new GiveawayInstance(Context.Channel.Id, Context.Message.Author.Id);
                instanceDictionary.Add(newInstance.channelId, newInstance);
                await newInstance.RunGiveaway(seconds, prize, Context, RemoveInstance);
            }
        }
        [Command("cancel"), Summary("Cancels the *running* game")]
        public async Task CancelGiveaway() {
            if (instanceDictionary.TryGetValue(Context.Channel.Id, out GiveawayInstance newInstance)) {
                await ReplyAsync("Cancelling game...");
                newInstance.CancelGame(Context.Message.Author.Id);
            }
        }
        private void RemoveInstance(ulong id) => instanceDictionary.Remove(id);
    }
}
