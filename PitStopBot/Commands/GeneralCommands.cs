using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace PitStopBot.Commands {
    public class GeneralCommands : ModuleBase<SocketCommandContext> {
        private readonly CommandService _commands;
        private readonly IServiceProvider _map;

        public GeneralCommands(IServiceProvider map, CommandService commands) {
            _commands = commands;
            _map = map;
        }
        [Command("ping"), Summary("Test to see if bot works.")]
        public async Task Pong() {
            await ReplyAsync($"pong");
        }
        [Command("help"), Summary("Returns commands information and usage.")]
        public async Task Help([Summary("module name for specific information")] string path = "") {
            EmbedBuilder output = new EmbedBuilder();
            if (path == "") {
                output.Title = "PitStopBot - help";
                output.Color = Color.Purple;
                foreach (var mod in _commands.Modules.Where(m => m.Parent == null)) {
                    AddHelp(mod, ref output);
                }

                output.Footer = new EmbedFooterBuilder {
                    Text = "Use 'help <module>' to get help with a module."
                };
            } else {
                var mod = _commands.Modules.FirstOrDefault(m => m.Name.Replace("Module", "").ToLower() == path.ToLower());
                if (mod == null) {
                    await ReplyAsync("No module could be found with that name.");
                    return;
                }

                output.Title = $"{mod.Name} - help";
                output.Color = Color.Magenta;
                output.Description = $"{mod.Summary}\n" +
                (!string.IsNullOrEmpty(mod.Remarks) ? $"({mod.Remarks})\n" : "") +
                (!mod.Aliases.Contains("") ? $"Prefix(es): {string.Join(",", mod.Aliases.Select(m => $"`{m}`"))}\n" : "") +
                (mod.Submodules.Any() ? $"Submodules: {string.Join(", ", mod.Submodules.Select(m => $"`{m.Name}`"))}\n" : "") + " ";
                AddCommands(mod, ref output);
            }

            await ReplyAsync("", embed: output.Build());
        }


        #region help command helpers
        public void AddHelp(ModuleInfo module, ref EmbedBuilder builder) {
            foreach (var sub in module.Submodules)
                AddHelp(sub, ref builder);
            builder.AddField(f => {
                f.Name = $"**{module.Name}**";
                f.Value = $"Submodules: {string.Join(", ", module.Submodules.Select(m => $"`{m.Name}`"))}" +
                $"\n" +
                $"Commands: {string.Join(", ", module.Commands.Select(x => $"`{x.Name}`"))}";
            });
        }

        public void AddCommands(ModuleInfo module, ref EmbedBuilder builder) {
            foreach (var command in module.Commands) {
                command.CheckPreconditionsAsync(Context, _map).GetAwaiter().GetResult();
                AddCommand(command, ref builder);
            }

        }

        public void AddCommand(CommandInfo command, ref EmbedBuilder builder) {
            builder.AddField(f => {
                f.Name = $"{new String('-', 30)}\n**{command.Name}**";
                f.Value = $"{command.Summary}\n" +
                (!string.IsNullOrEmpty(command.Remarks) ? $"({command.Remarks})\n" : "") +
                (command.Aliases.Any() ? $"**Aliases:** {string.Join(", ", command.Aliases.Select(x => $"`{x}`"))}\n" : "") +
                $"**Usage:** `{GetPrefix(command)} {GetAliases(command)}`\n" +
                $"**Parameters:** \n{GetParameters(command)}";
            });
        }

        public string GetAliases(CommandInfo command) {
            StringBuilder output = new StringBuilder();
            if (!command.Parameters.Any())
                return output.ToString();
            foreach (var param in command.Parameters) {
                if (param.IsOptional)
                    output.Append($"[{param.Name} = {param.DefaultValue}] ");
                else if (param.IsMultiple)
                    output.Append($"|{param.Name}| ");
                else if (param.IsRemainder)
                    output.Append($"...{param.Name} ");
                else
                    output.Append($"<{param.Name}> ");
            }
            return output.ToString();
        }
        public string GetParameters(CommandInfo command) {
            StringBuilder output = new StringBuilder();
            if (!command.Parameters.Any())
                return output.ToString();
            foreach (var param in command.Parameters) {
                output.Append($"`{param.Name}` = {param.Summary}\n");
            }
            return output.ToString();
        }
        public string GetPrefix(CommandInfo command) {
            var output = "";
            output += $"{command.Aliases.FirstOrDefault()} ";
            return output;
        }
        public string GetPrefix(ModuleInfo module) {
            string output = "";
            if (module.Parent != null)
                output = $"{GetPrefix(module.Parent)}{output}";
            if (module.Aliases.Any())
                output += string.Concat(module.Aliases.FirstOrDefault(), " ");
            return output;
        }
        #endregion
    }
}