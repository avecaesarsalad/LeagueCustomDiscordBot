using System.Text;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LeagueCustomBot.resx;
using LeagueCustomBot.teamcreator;

namespace LeagueCustomBot.commands;

public static class StaticCommands
{
    public static async Task StartLobby(DiscordInteraction interaction)
    {
        await Functions.DeleteLastMessages(interaction);
        var lobbyStarted = TeamCreator.Instance.StartLobby();

        if (lobbyStarted)
        {
            var builder = new DiscordInteractionResponseBuilder()
                .WithContent(BotResources.LobbyStarted);

            await interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, builder);
        }
        else
        {
            var showLobby = TeamCreator.Instance.PrintLobbyMembers();
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(BotResources.LobbyAlreadyRunning);
            stringBuilder.AppendLine(showLobby);

            await interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent(stringBuilder.ToString()));
        }
    }
    
    public static async Task RollTeams(DiscordInteraction interaction)
    {
        await Functions.DeleteLastMessages(interaction);
        var builder = new DiscordInteractionResponseBuilder();

        if (!TeamCreator.Instance.LobbyRunning())
        {
            builder = builder.WithContent(BotResources.LobbyNotRunning)
                .AddComponents(Buttons.StartLobbyButton);
        }
        else if (!TeamCreator.Instance.LobbyFull())
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(BotResources.LobbyNotFull);
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine(TeamCreator.Instance.PrintLobbyMembers());

            builder = builder.WithContent(stringBuilder.ToString());
        }
        else
        {
            var rolled = TeamCreator.Instance.Roll();
            if (rolled)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(BotResources.Rolled);
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine(TeamCreator.Instance.PrintTeams());
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine(BotResources.WantToReroll);

                builder = builder.WithContent(stringBuilder.ToString())
                    .AddComponents(Buttons.RerollTeamsButton);
            }
            else
            {
                builder = builder.WithContent(BotResources.Error);
            }
        }

        await interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, builder);
    }
}