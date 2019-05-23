﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace PitStopBot.Utils {
    public class GiveawayUtils {
        private EmbedBuilder MyEmbedBuilder = new EmbedBuilder();
        private EmbedFieldBuilder MyEmbedField = new EmbedFieldBuilder();
        private readonly Emoji dice = new Emoji("🎲");
        private readonly Emoji trophy = new Emoji("🏆");
        private Random rand = new Random();
        public GiveawayUtils() {
        }
        public async Task RunGiveaway(int seconds, string prize, ICommandContext context) {

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
                await Task.Delay(1000);
                Console.WriteLine(seconds);
                seconds--;
                var newMessage = await message.Channel.GetMessageAsync(message.Id) as IUserMessage;
                var embed2 = new EmbedBuilder();
                embed2.AddField(Name);
                embed2.WithColor(Color.DarkBlue);

                MyEmbedField.WithValue($"Free ***{prize}***\nReact with {dice} to win!\nTime remaining: {seconds} seconds");

                await newMessage.ModifyAsync(m => m.Embed = embed2.Build());

            }


            //Adds users to list and randomly selects winner
            await message.RemoveReactionAsync(dice, message.Author);
            var temp = await message.GetReactionUsersAsync(dice, 500).FlattenAsync();

            if (temp.Any()) {
                IUser winner = temp.ElementAt(rand.Next(temp.Count()));
                var message3 = await message.Channel.GetMessageAsync(message.Id) as IUserMessage;
                var embed3 = new EmbedBuilder();
                embed3.AddField(Name);
                embed3.WithColor(new Color(255, 255, 0));

                MyEmbedField.WithValue($"***Congratulations***! {winner.Mention} You won ***{prize}***!");
                await message3.ModifyAsync(m => m.Embed = embed3.Build());
                await message3.AddReactionAsync(trophy);
            } else {
                var message4 = await message.Channel.GetMessageAsync(message.Id) as IUserMessage;
                var embed4 = new EmbedBuilder();
                embed4.AddField(Name);
                embed4.WithColor(new Color(255, 255, 0));

                MyEmbedField.WithValue("There are no winners today");
                await message4.ModifyAsync(m => m.Embed = embed4.Build());
                await message4.AddReactionAsync(trophy);

            }

        }
    }
}