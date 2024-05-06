using System;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using LeagueCustomBot.commands;
using LeagueCustomBot.json;
using LeagueCustomBot.resx;
using LeagueCustomBot.teamcreator;

namespace LeagueCustomBot
{
    internal static class Program
    {
        private static DiscordClient Client { get; set; } = null!;
        private static CommandsNextExtension Commands { get; set; } = null!;
        [Obsolete("Obsolete")] private static SlashCommandsExtension SlashCommands { get; set; } = null!;

        [Obsolete("Obsolete")]
        static async Task Main(string[] args)
        {
            var jsonReader = new JsonReader();
            await jsonReader.ReadJson();

            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = jsonReader.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
            };

            Client = new DiscordClient(discordConfig);

            Client.ComponentInteractionCreated += Client_ComponentInteractionCreated;

            SlashCommands = Client.UseSlashCommands();

            SlashCommands.RegisterCommands<BasicCommands>();

        await Client.ConnectAsync();
        await Task.Delay(-1);
    }

        private static async Task Client_ComponentInteractionCreated(DiscordClient sender,
            ComponentInteractionCreateEventArgs args)
        {
            switch (args.Interaction.Data.CustomId)
            {
                case "start":
                    await StaticCommands.StartLobby(args.Interaction);
                    break;
                case "roll":
                    await StaticCommands.RollTeams(args.Interaction);
                    break;
                default:
                    break;
            }
        }
    }
}
