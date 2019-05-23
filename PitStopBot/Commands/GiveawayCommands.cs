using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using PitStopBot.Utils;

namespace PitStopBot.Commands {
    [Group("giveaway")]
    public class Giveaway : ModuleBase {
        GiveawayUtils gUtils;
        public Giveaway() {
            gUtils = new GiveawayUtils();
        }

        [Command("randomdraw"), Summary("Users will react to the message and the bot will randomly pick a user to win.")]
        public async Task RandomDraw([Summary("seconds to wait for the giveaway")] int seconds,
        [Summary("What the prize is for the winner."), Remainder] string prize) {
            await gUtils.RunGiveaway(seconds, prize, Context);

        }
    }
}
