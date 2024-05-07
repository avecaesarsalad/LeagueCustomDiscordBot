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
        var lobbyStarted = TeamCreator.Instance.StartLobby(interaction.Guild.Members[interaction.User.Id].DisplayName);

        if (lobbyStarted)
        {
            var builder = new DiscordInteractionResponseBuilder()
                .WithContent(string.Format(BotResources.LobbyStarted, interaction.Guild.Members[interaction.User.Id].DisplayName));

            await interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, builder);
        }
        else
        {
            if (TeamCreator.Instance.GetRolled())
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(BotResources.Rolled);
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(TeamCreator.Instance.PrintTeams());
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(BotResources.WantToReroll);

                var builder = new DiscordInteractionResponseBuilder().WithContent(stringBuilder.ToString())
                    .AddComponents(Buttons.RerollTeamsButton)
                    .AddComponents(Buttons.MoveTeamsButton);

                await interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                    builder);
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
    }

    public static async Task RollTeams(DiscordInteraction interaction)
    {
        await Functions.DeleteLastMessages(interaction);
        var builder = new DiscordInteractionResponseBuilder();

        if (interaction.Guild.Members[interaction.User.Id].DisplayName != TeamCreator.Instance.GetLobbyMasterName())
        {
            var stringBuilder = new StringBuilder();
            if (TeamCreator.Instance.GetRolled())
            {
                stringBuilder.AppendLine(string.Format(BotResources.OnlyRollMasterCanRoll,
                    TeamCreator.Instance.GetLobbyMasterName()));
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine(TeamCreator.Instance.PrintTeams());
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine(BotResources.WantToReroll);

                builder = builder.WithContent(stringBuilder.ToString())
                    .AddComponents(Buttons.RerollTeamsButton)
                    .AddComponents(Buttons.MoveTeamsButton);
            }
            else
            {
                stringBuilder.AppendLine(string.Format(BotResources.OnlyRollMasterCanRoll,
                    TeamCreator.Instance.GetLobbyMasterName()));
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine(TeamCreator.Instance.PrintLobbyMembers());

                builder = builder.WithContent(stringBuilder.ToString())
                    .AddComponents(Buttons.RollTeamsButton);
            }
        }
        else if (!TeamCreator.Instance.LobbyRunning())
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
                TeamCreator.Instance.SetRolled(rolled);

                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(BotResources.Rolled);
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine(TeamCreator.Instance.PrintTeams());
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine(BotResources.WantToReroll);

                builder = builder.WithContent(stringBuilder.ToString())
                    .AddComponents(Buttons.RerollTeamsButton)
                    .AddComponents(Buttons.MoveTeamsButton);
            }
            else
            {
                TeamCreator.Instance.SetRolled(false);
                builder = builder.WithContent(BotResources.Error);
            }
        }

        await interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, builder);
    }

    public static async Task MoveTeamsToChannels(DiscordInteraction interaction)
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
        else if (!TeamCreator.Instance.GetRolled())
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(BotResources.LobbyNotRolled);
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine(TeamCreator.Instance.PrintLobbyMembers());

            builder = builder.WithContent(stringBuilder.ToString())
                .AddComponents(Buttons.RollTeamsButton);
        }
        else if (!ChannelManager.GetInstance().RedTeamChannelId.HasValue ||
                 !ChannelManager.GetInstance().BlueTeamChannelId.HasValue)
        {
            builder = builder.WithContent(BotResources.ChannelsNeedToBeSetup);
        }
        else
        {
            var redChannel = interaction.Guild.GetChannel(ChannelManager.GetInstance().RedTeamChannelId!.Value);
            var blueChannel = interaction.Guild.GetChannel(ChannelManager.GetInstance().BlueTeamChannelId!.Value);

            if (redChannel is null || blueChannel is null)
            {
                builder = builder.WithContent(BotResources.ChannelsNeedToBeSetup);
            }
            else
            {
                if (TeamCreator.Instance.GetLobbyMasterName() ==
                    interaction.Guild.Members[interaction.User.Id].DisplayName)
                {
                    var redTeamTasks = TeamCreator.Instance.GetRedTeam().Select(async player =>
                    {
                        var member = await interaction.Guild.GetMemberAsync(player.Id);
                        if (member != null)
                        {
                            await member.ModifyAsync(x => x.VoiceChannel = redChannel);
                        }
                    });

                    var blueTeamTasks = TeamCreator.Instance.GetBlueTeam().Select(async player =>
                    {
                        var member = await interaction.Guild.GetMemberAsync(player.Id);
                        if (member != null)
                        {
                            await member.ModifyAsync(x => x.VoiceChannel = blueChannel);
                        }
                    });

                    await Task.WhenAll(redTeamTasks.Concat(blueTeamTasks));   
                }

                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(BotResources.Rolled);
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(TeamCreator.Instance.PrintTeams());
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(BotResources.WantToReroll);

                builder = builder.WithContent(stringBuilder.ToString())
                    .AddComponents(Buttons.RerollTeamsButton)
                    .AddComponents(Buttons.MoveTeamsButton);
            }
        }

        await interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, builder);
    }
}