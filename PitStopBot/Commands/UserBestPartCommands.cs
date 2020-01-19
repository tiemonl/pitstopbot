using System;
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
        FRONT_DUR_POWER,
        BODY_DUR_SPEED,
        WHEELS_POWER_STEERING,
        BACK_SPEED_STEERING,
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
        [Group("best"), Summary("Best is used to determine which part has the best specified attribute")]
        class Best : UserInfo {

            private readonly Logger logger = new Logger();

            [Command("front", RunMode = RunMode.Async), Alias("f"), Summary("determines which front part has the best specified attribute")]
            public async Task GetBestFront(
                [Summary("which attribute to determine is best (Durability | Power | Both)")]string attribute,
                    [Summary("User's eth adress")] string addressInput) =>
                await GetBestPartAsync(addressInput, PartType.FRONT, attribute);

            [Command("rear", RunMode = RunMode.Async), Alias("r"), Summary("determines which rear part has the best specified attribute")]
            public async Task GetBestRear(
                [Summary("which attribute to determine is best (Speed | Steering | Both)")]string attribute,
                    [Summary("User's eth adress")] string addressInput) =>
                await GetBestPartAsync(addressInput, PartType.BACK, attribute);

            [Command("Wheels", RunMode = RunMode.Async), Alias("w"), Summary("determines which wheel part has the best specified attribute")]
            public async Task GetBestWheels(
                [Summary("which attribute to determine is best (Power | Steering | Both)")]string attribute,
                    [Summary("User's eth adress")] string addressInput) =>
                await GetBestPartAsync(addressInput, PartType.WHEELS, attribute);

            [Command("body", RunMode = RunMode.Async), Alias("b"), Summary("determines which body part has the best specified attribute")]
            public async Task GetBestBody(
                [Summary("which attribute to determine is best (Durability | Steering | Both)")]string attribute,
                    [Summary("User's eth adress")] string addressInput) =>
                await GetBestPartAsync(addressInput, PartType.BODY, attribute);


            private async Task GetBestPartAsync(string addressInput, PartType partType, string attribute) {
                var address = await GetFormattedAddress(addressInput);
                Inventory inv = await userUtils.GetInventory(address);
                var parts = inv.parts;

                var attr = attribute.ToLower().StartsWith('b') ? ConvertToTypeName(partType) : attribute;
                var partAttribute = GetPartAttribute(attr);

                if (partAttribute == PartAttribute.NONE) {
                    MyEmbedBuilder.AddField("Attribute not found", attribute);
                    MyEmbedBuilder.WithColor(Color.Red);
                } else {
                    var partsByType = GetPartsByType(parts, partType);

                    var bestPartAttribute = GetBestAttribute(partsByType, partAttribute);

                    SetUpResponse(bestPartAttribute, partAttribute);

                    MyEmbedBuilder.WithTitle(ToSentenceCase($"Best {partType.ToString("G")} | {attribute}"));
                    MyEmbedBuilder.WithColor(Color.DarkTeal);
                }
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
                } else if (attribute == PartAttribute.FRONT_DUR_POWER) {
                    MyEmbedBuilder.AddField("Durability", part.details.durability, true);
                    MyEmbedBuilder.AddField("Power", part.details.power, true);
                } else if (attribute == PartAttribute.BACK_SPEED_STEERING) {
                    MyEmbedBuilder.AddField("Speed", part.details.speed, true);
                    MyEmbedBuilder.AddField("Steering", part.details.steering, true);
                } else if (attribute == PartAttribute.WHEELS_POWER_STEERING) {
                    MyEmbedBuilder.AddField("Power", part.details.power, true);
                    MyEmbedBuilder.AddField("Steering", part.details.steering, true);
                } else if (attribute == PartAttribute.BODY_DUR_SPEED) {
                    MyEmbedBuilder.AddField("Durability", part.details.durability, true);
                    MyEmbedBuilder.AddField("Speed", part.details.speed, true);
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
                } else if (attribute == PartAttribute.FRONT_DUR_POWER) {
                    return parts.Aggregate((p1, p2) => (p1.details.durability + p1.details.power) > (p2.details.durability + p2.details.power) ? p1 : p2);
                } else if (attribute == PartAttribute.BACK_SPEED_STEERING) {
                    return parts.Aggregate((p1, p2) => (p1.details.speed + p1.details.steering) > (p2.details.speed + p2.details.steering) ? p1 : p2);
                } else if (attribute == PartAttribute.WHEELS_POWER_STEERING) {
                    return parts.Aggregate((p1, p2) => (p1.details.steering + p1.details.power) > (p2.details.steering + p2.details.power) ? p1 : p2);
                } else if (attribute == PartAttribute.BODY_DUR_SPEED) {
                    return parts.Aggregate((p1, p2) => (p1.details.durability + p1.details.speed) > (p2.details.durability + p2.details.speed) ? p1 : p2);
                } else {
                    return parts.Aggregate((p1, p2) => p1.details.weight < p2.details.weight ? p1 : p2);
                }
            }

            private PartType GetPartType(string type) {
                foreach (string t in Enum.GetNames(typeof(PartType)))
                    if (t.Equals(type, StringComparison.OrdinalIgnoreCase))
                        return (PartType)Enum.Parse(typeof(PartType), t);
                return PartType.NONE;
            }

            private PartAttribute GetPartAttribute(string attribute) {
                foreach (string attr in Enum.GetNames(typeof(PartAttribute)))
                    if (attr.Equals(attribute, StringComparison.OrdinalIgnoreCase))
                        return (PartAttribute)Enum.Parse(typeof(PartAttribute), attr);
                return PartAttribute.NONE;
            }

            private string ConvertToTypeName(PartType partType) {
                foreach (string partAttr in Enum.GetNames(typeof(PartAttribute)))
                    if (partAttr.Contains(partType.ToString("G")))
                        return partAttr;
                return "NONE";
            }

            private string ToSentenceCase(string input) {
                if (input.Length < 1)
                    return input;

                string sentence = input.ToLower();
                return sentence[0].ToString().ToUpper() +
                   sentence.Substring(1);
            }
        }
    }
}