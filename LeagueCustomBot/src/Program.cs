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
        private static ChannelManager ChannelManagerInstance { get; set; } = null!;
        [Obsolete("Obsolete")] private static SlashCommandsExtension SlashCommands { get; set; } = null!;

        [Obsolete("Obsolete")]
        static async Task Main(string[] args)
        {
            var jsonReader = new JsonReader();
            await jsonReader.ReadJson(JsonTypes.Config);
            await jsonReader.ReadJson(JsonTypes.Channels);

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

            ChannelManagerInstance = ChannelManager.GetInstance();

            if (jsonReader is { BlueTeamChannelId: not null, RedTeamChannelId: not null, BaseChannelId: not null, })
            {
                ChannelManagerInstance.BlueTeamChannelId = jsonReader.BlueTeamChannelId.Value;
                ChannelManagerInstance.RedTeamChannelId = jsonReader.RedTeamChannelId.Value;
                ChannelManagerInstance.BaseChannelId = jsonReader.BaseChannelId.Value;
            }
            
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
                case "reroll":
                    await StaticCommands.RollTeams(args.Interaction);
                    break;
                case "move":
                    await StaticCommands.MoveTeamsToChannels(args.Interaction);
                    break;
                case "moveback":
                    await StaticCommands.MoveTeamsBackToBaseChannel(args.Interaction);
                    break;
                default:
                    break;
            }
        }
    }
}