using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using PitStopBot;
using PitStopBot.Commands;
using PitStopBot.Objects;

namespace Commands {

    enum PartAttribute {
        SPEED,
        WEIGHT,
        DURABILITY,
        POWER,
        STEERING,
        NONE
    }

    enum PartType {
        BODY,
        FRONT,
        WHEELS,
        BACK,
        NONE
    }


    public class UserBestPartCommands : UserInfo {

        private readonly Logger logger = new Logger();

        [Command("best", RunMode = RunMode.Async), Alias("b"), Summary("Best is used to determine which part has the best specified attribute")]
        public async Task GetBestFront(
            [Summary("which part of the car to determine is best (Front | Body | Back | Wheels)")]string type,
            [Summary("which attribute to determine is best (Speed | Weight | Durability | Power | Steering)")]string attribute,
                [Summary("User's eth adress")] string addressInput) {

            var address = await GetFormattedAddress(addressInput);
            Inventory inv = await userUtils.GetInventory(address);

            var parts = inv.parts;

            var partType = GetPartType(type);
            var partAttribute = GetPartAttribute(attribute);

            if (partType == PartType.NONE) {
                MyEmbedBuilder.AddField("Part not found", type);
                MyEmbedBuilder.WithColor(Color.Red);
            } else if (partAttribute == PartAttribute.NONE) {
                MyEmbedBuilder.AddField("Attribute not found", attribute);
                MyEmbedBuilder.WithColor(Color.Red);
            } else {
                var partsByType = GetPartsByType(parts, partType);

                var bestPartAttribute = GetBestAttribute(partsByType, partAttribute);

                SetUpResponse(bestPartAttribute, partAttribute);

                MyEmbedBuilder.WithTitle($"Best {type} {attribute}");
                MyEmbedBuilder.WithColor(Color.DarkTeal);
            }
            await logger.Log(new LogMessage(LogSeverity.Critical, "", "test"));
            await ReplyAsync(embed: MyEmbedBuilder.Build());
        }

        private void SetUpResponse(Part part, PartAttribute attribute) {
            MyEmbedBuilder.AddField("Part", part.id, true);
            MyEmbedBuilder.AddField("Brand", part.details.brand, true);
            MyEmbedBuilder.AddField("Model", part.details.model, true);
            if (attribute == PartAttribute.DURABILITY) {
                MyEmbedBuilder.AddField("Durability", part.details.durability, true);
            } else if (attribute == PartAttribute.POWER) {
                MyEmbedBuilder.AddField("Power", part.details.power, true);
            } else if (attribute == PartAttribute.SPEED) {
                MyEmbedBuilder.AddField("Speed", part.details.speed, true);
            } else if (attribute == PartAttribute.STEERING) {
                MyEmbedBuilder.AddField("Steering", part.details.steering, true);
            } else {
                MyEmbedBuilder.AddField("Weight", part.details.weight, true);
            }
            MyEmbedBuilder.WithThumbnailUrl(part.image);
        }

        private List<Part> GetPartsByType(List<Part> parts, PartType type) {
            if (type == PartType.FRONT) {
                return parts.Where(p => p.details.type.ToLower().StartsWith("f")).ToList();
            } else if (type == PartType.BACK) {
                return parts.Where(p => p.details.type.ToLower().Contains("back")).ToList();
            } else if (type == PartType.BODY) {
                return parts.Where(p => p.details.type.ToLower().Contains("body")).ToList();
            } else {
                return parts.Where(p => p.details.type.ToLower().StartsWith("w")).ToList();
            }
        }

        private Part GetBestAttribute(List<Part> parts, PartAttribute attribute) {
            if (attribute == PartAttribute.DURABILITY) {
                return parts.Aggregate((p1, p2) => p1.details.durability > p2.details.durability ? p1 : p2);
            } else if (attribute == PartAttribute.POWER) {
                return parts.Aggregate((p1, p2) => p1.details.power > p2.details.power ? p1 : p2);
            } else if (attribute == PartAttribute.SPEED) {
                return parts.Aggregate((p1, p2) => p1.details.speed > p2.details.speed ? p1 : p2);
            } else if (attribute == PartAttribute.STEERING) {
                return parts.Aggregate((p1, p2) => p1.details.steering > p2.details.steering ? p1 : p2);
            } else {
                return parts.Aggregate((p1, p2) => p1.details.weight < p2.details.weight ? p1 : p2);
            }
        }

        private PartType GetPartType(string type) =>
                type.ToLower().Equals("front") ? PartType.FRONT :
                type.ToLower().Equals("back") ? PartType.BACK :
                type.ToLower().Equals("body") ? PartType.BODY :
                type.ToLower().Equals("wheels") ? PartType.WHEELS :
                PartType.NONE;

        private PartAttribute GetPartAttribute(string attribute) =>
            attribute.ToLower().Equals("speed") ? PartAttribute.SPEED :
            attribute.ToLower().Equals("weight") ? PartAttribute.WEIGHT :
            attribute.ToLower().Equals("durability") ? PartAttribute.DURABILITY :
            attribute.ToLower().Equals("power") ? PartAttribute.POWER :
            attribute.ToLower().Equals("steering") ? PartAttribute.STEERING :
            PartAttribute.NONE;
    }
}
