using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace PitStopBot.Utils {
    public class GiveawayInstance {
        public readonly ulong channelId;
        public readonly ulong creatorId;
        private EmbedBuilder MyEmbedBuilder = new EmbedBuilder();
        private EmbedFieldBuilder MyEmbedField = new EmbedFieldBuilder();
        private readonly Emoji dice = new Emoji("🎲");
        private readonly Emoji trophy = new Emoji("🏆");
        private Random rand = new Random();
        private bool isCancel;
        public GiveawayInstance(ulong _channelId, ulong _creatorId) {
            channelId = _channelId;
            creatorId = _creatorId;
            isCancel = false;
        }

        public async Task RunGiveaway(int seconds, string prize, ICommandContext context, Action<ulong> removeInstance) {
            //Starts the Embeded Message
            MyEmbedBuilder.WithColor(Color.DarkBlue);
            var Name = MyEmbedField.WithName(":game_die: **GIVEAWAY**  :game_die:");
            MyEmbedField.WithIsInline(true);

            var msg = MyEmbedField.WithValue($"Prize: ***{prize}***\nReact with {dice} to win!\nTime remaining: {seconds} seconds");

            //Sends message
            MyEmbedBuilder.AddField(MyEmbedField);
            var message = await context.Channel.SendMessageAsync("", false, MyEmbedBuilder.Build());

            //Reacts to message
            await message.AddReactionAsync(dice);

            //Begins countdown and edits embeded field every hour, minute, or second
            while (seconds > 0) {
                await Task.Delay(5000);
                seconds -= 5;
                var countdownEmbed = new EmbedBuilder();
                countdownEmbed.AddField(Name);
                countdownEmbed.WithColor(Color.DarkBlue);
                MyEmbedField.WithValue($"Prize: ***{prize}***\nReact with {dice} to win!\nTime remaining: {seconds} seconds");
                if (isCancel) {
                    MyEmbedField.WithValue($"GIVEAWAY CANCELLED!");
                    await message.ModifyAsync(m => m.Embed = countdownEmbed.Build());
                    removeInstance(channelId);
                    return;
                }
                await message.ModifyAsync(m => m.Embed = countdownEmbed.Build());
            }

            //Adds users to list and randomly selects winner
            await message.RemoveReactionAsync(dice, message.Author);
            var temp = await message.GetReactionUsersAsync(dice, 500).FlattenAsync();

            var finalEmbed = new EmbedBuilder();
            finalEmbed.AddField(Name);
            finalEmbed.WithColor(new Color(255, 255, 0));

            if (temp.Any()) {
                IUser winner = temp.ElementAt(rand.Next(temp.Count()));
                MyEmbedField.WithValue($"***Congratulations***! {winner.Mention} You won ***{prize}***!");
                await message.ModifyAsync(m => m.Embed = finalEmbed.Build());
                await message.AddReactionAsync(trophy);
            } else {
                MyEmbedField.WithValue("There are no winners today");
                await message.ModifyAsync(m => m.Embed = finalEmbed.Build());
                await message.AddReactionAsync(trophy);

            }
            removeInstance(channelId);
        }

        public void CancelGame(ulong senderId) {
            isCancel |= senderId == creatorId;
        }
    }
}
