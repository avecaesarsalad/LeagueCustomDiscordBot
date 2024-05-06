using System.Text;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using LeagueCustomBot.resx;
using LeagueCustomBot.teamcreator;

namespace LeagueCustomBot.commands;

public class BasicCommands : ApplicationCommandModule
{
    [SlashCommand("start-lobby", "starts a new lobby")]
    public async Task StartLobby(InteractionContext ctx)
    {
        await StaticCommands.StartLobby(ctx.Interaction);
    }

    [SlashCommand("restart-lobby", "restarts the lobby")]
    public async Task RestartLobby(InteractionContext ctx)
    {
        await Functions.DeleteLastMessages(ctx.Interaction);
        TeamCreator.Instance.RestartLobby();

        var builder = new DiscordInteractionResponseBuilder()
            .WithContent(BotResources.LobbyRestarted);

        await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, builder);
    }

    [SlashCommand("join-lobby", "Joins lobby")]
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
            var player = new Player
            {
                Name = ctx.Interaction.User.GlobalName,
                FirstRole = role1,
                SecondRole = role2,
                Rank = rank,
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

    [SlashCommand("remove-player", "Removes player from lobby")]
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

    [SlashCommand("roll-teams", "Rolls Teams!")]
    public async Task RollTeams(InteractionContext ctx)
    {
        await StaticCommands.RollTeams(ctx.Interaction);
    }
}