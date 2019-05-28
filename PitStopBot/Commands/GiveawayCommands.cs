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
        public static Dictionary<ulong, GiveawayInstance> instanceDictionary;
        static Giveaway() {
            instanceDictionary = new Dictionary<ulong, GiveawayInstance>();
        }

        [Command("randomdraw", RunMode = RunMode.Async), Summary("Users will react to the message and the bot will randomly pick a user to win.")]
        public async Task RandomDraw([Summary("seconds to wait for the giveaway")] int seconds,
        [Summary("What the prize is for the winner."), Remainder] string prize) {
            GiveawayInstance newInstance;
            if (instanceDictionary.TryGetValue(Context.Channel.Id, out newInstance)) {
                await ReplyAsync("Game already running. Please wait until it is over");
            }
            else{
                newInstance = new GiveawayInstance(Context.Channel.Id);
                instanceDictionary.Add(newInstance.channelId, newInstance); 
                await newInstance.RunGiveaway(seconds, prize, Context, RemoveInstance);
            }
        }

        private void RemoveInstance(ulong id) => instanceDictionary.Remove(id);
        
    }
}
