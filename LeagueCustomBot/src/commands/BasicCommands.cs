using System.Text;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LeagueCustomBot.json;
using LeagueCustomBot.resx;
using LeagueCustomBot.teamcreator;

namespace LeagueCustomBot.commands;

public class BasicCommands : ApplicationCommandModule
{
    [SlashCommand("start", "starts a new lobby")]
    public async Task StartLobby(InteractionContext ctx)
    {
        await StaticCommands.StartLobby(ctx.Interaction);
    }

    [SlashCommand("restart", "restarts the lobby")]
    public async Task RestartLobby(InteractionContext ctx)
    {
        await Functions.DeleteLastMessages(ctx.Interaction);
        TeamCreator.Instance.RestartLobby(ctx.Interaction.Guild.Members[ctx.Interaction.User.Id].DisplayName);

        var builder = new DiscordInteractionResponseBuilder()
            .WithContent(string.Format(BotResources.LobbyRestarted, ctx.Interaction.Guild.Members[ctx.Interaction.User.Id].DisplayName));

        await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, builder);
    }

    [SlashCommand("join", "Joins lobby")]
    public async Task JoinLobby(InteractionContext ctx, [Option("Rank", "Type in your rank")] Rank rank,
        [Option("FirstRole", "Select your first role")]
        Role role1,
        [Option("SecondRole", "Select your second role")]
        Role role2)
    {
        await Functions.DeleteLastMessages(ctx.Interaction);

        var builder = new DiscordInteractionResponseBuilder();

        if (!TeamCreator.Instance.LobbyRunning())
        {
            builder = builder.WithContent(BotResources.LobbyNotRunning)
                .AddComponents(Buttons.StartLobbyButton);
            TeamCreator.Instance.SetRolled(false);
        }
        else if (TeamCreator.Instance.LobbyFull())
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(BotResources.LobbyFullAlready);
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine(TeamCreator.Instance.PrintLobbyMembers());

            builder = builder.WithContent(stringBuilder.ToString())
                .AddComponents(Buttons.RollTeamsButton);
        }
        else
        {
            TeamCreator.Instance.SetRolled(false);

            var player = new Player
            {
                Name = ctx.Interaction.Guild.Members[ctx.Interaction.User.Id].DisplayName ?? "NoNameFound",
                FirstRole = role1,
                SecondRole = role2,
                Rank = rank,
                Id = ctx.Interaction.User.Id,
            };

            var added = TeamCreator.Instance.AddPlayerToList(player);

            if (added)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(string.Format(BotResources.PlayerAdded, player.Name));
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine(TeamCreator.Instance.PrintLobbyMembers());
                builder = builder.WithContent(stringBuilder.ToString());

                if (TeamCreator.Instance.LobbyFull())
                {
                    builder.AddComponents(Buttons.RollTeamsButton);
                }
            }
            else
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(string.Format(BotResources.PlayerAlreadyInLobby, player.Name));
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine(TeamCreator.Instance.PrintLobbyMembers());
                builder = builder.WithContent(stringBuilder.ToString());
            }
        }

        await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, builder);
    }

    [SlashCommand("remove", "Removes player from lobby")]
    public async Task RemovePlayer(InteractionContext ctx,
        [Option("Name", "Type in the name of the person to be removed")]
        string name)
    {
        await Functions.DeleteLastMessages(ctx.Interaction);

        var builder = new DiscordInteractionResponseBuilder();

        if (!TeamCreator.Instance.LobbyRunning())
        {
            builder = builder.WithContent(BotResources.LobbyNotRunning)
                .AddComponents(Buttons.StartLobbyButton);
        }
        else
        {
            var removed = TeamCreator.Instance.RemovePlayerFromList(name);

            if (removed)
            {
                TeamCreator.Instance.SetRolled(false);

                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(string.Format(BotResources.PlayerRemoved, name));
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine(TeamCreator.Instance.PrintLobbyMembers());

                builder = builder.WithContent(stringBuilder.ToString());
            }
            else
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine(string.Format(BotResources.PlayerNotFound, name));
                stringBuilder.AppendLine("");
                stringBuilder.AppendLine(TeamCreator.Instance.PrintLobbyMembers());
                builder = builder.WithContent(stringBuilder.ToString());
            }
        }

        await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, builder);
    }

    [SlashCommand("roll", "Rolls Teams!")]
    public async Task RollTeams(InteractionContext ctx)
    {
        await StaticCommands.RollTeams(ctx.Interaction);
    }

    [SlashCommand("change", "Changes your roles/rating!")]
    public async Task ChangeRolesAndRating(InteractionContext ctx, [Option("Rank", "Type in your rank")] Rank rank,
        [Option("FirstRole", "Select your first role")]
        Role role1,
        [Option("SecondRole", "Select your second role")]
        Role role2)
    {
        await Functions.DeleteLastMessages(ctx.Interaction);
        
        TeamCreator.Instance.SetRolled(false);

        var newPlayerInformation = new Player
        {
            Name = ctx.Interaction.Guild.Members[ctx.Interaction.User.Id].DisplayName,
            FirstRole = role1,
            SecondRole = role2,
            Rank = rank,
        };

        var changedPlayer = TeamCreator.Instance.ChangePlayerInformation(newPlayerInformation);

        var builder = new DiscordInteractionResponseBuilder();

        if (changedPlayer)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Format(BotResources.PlayerInformationChanged,
                ctx.Interaction.Guild.Members[ctx.Interaction.User.Id].DisplayName));
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine(TeamCreator.Instance.PrintLobbyMembers());

            builder = builder.WithContent(stringBuilder.ToString());
        }
        else
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Format(BotResources.PlayerNotFound, ctx.Interaction.Guild.Members[ctx.Interaction.User.Id].DisplayName));
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine(TeamCreator.Instance.PrintLobbyMembers());

            builder = builder.WithContent(stringBuilder.ToString());
        }

        if (TeamCreator.Instance.LobbyFull())
        {
            builder = builder.AddComponents(Buttons.RerollTeamsButton);
        }

        await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, builder);
    }

    [SlashCommand("move-teams", "Moves Teams to channels")]
    public async Task MoveTeamsToChannels(InteractionContext ctx)
    {
        await StaticCommands.MoveTeamsToChannels(ctx.Interaction);
    }

    [SlashCommand("setup-channels", "Setups the channels for the teams!")]
    public async Task MoveTeamsToChannels(InteractionContext ctx,
        [Option("BaseChannel", "Base Channel")] DiscordChannel baseChannel,
        [Option("BlueChannel", "Channel for blue team")] DiscordChannel blueChannel,
        [Option("RedChannel", "Channel for red team")] DiscordChannel redChannel)
    {
        await Functions.DeleteLastMessages(ctx.Interaction);
        
        var builder = new DiscordInteractionResponseBuilder();

        if (redChannel.Type != DiscordChannelType.Voice || blueChannel.Type != DiscordChannelType.Voice || baseChannel.Type != DiscordChannelType.Voice)
        {
            builder = builder.WithContent(BotResources.ChannelsNeedToBeVoiceChannels);
        }
        else
        {
            builder = builder.WithContent(string.Format(BotResources.ChannelsSpecified, blueChannel.Id,
                redChannel.Id, baseChannel.Id));

            ChannelManager.GetInstance().BlueTeamChannelId = blueChannel.Id;
            ChannelManager.GetInstance().RedTeamChannelId = redChannel.Id;
            ChannelManager.GetInstance().BaseChannelId = baseChannel.Id;

            var jsonReader = new JsonReader();
            await jsonReader.WriteToJson(redChannel.Id, blueChannel.Id, baseChannel.Id);
        }

        await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, builder);
    }
    
    [SlashCommand("move-back", "Moves Teams back to base channel")]
    public async Task MoveTeamsBack(InteractionContext ctx)
    {
        await StaticCommands.MoveTeamsBackToBaseChannel(ctx.Interaction);
    }
    
}