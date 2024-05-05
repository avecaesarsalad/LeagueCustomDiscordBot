using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LeagueCustomBot.teamcreator;

namespace LeagueCustomBot.commands;

public class BasicCommands : ApplicationCommandModule
{
    [SlashCommand("start-lobby", "starts a new lobby")]
    public async Task StartLobby(InteractionContext ctx)
    {
        await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent(TeamCreator.Instance.StartLobby()));
    }

    [SlashCommand("restart-lobby", "restarts the lobby")]
    public async Task RestartLobby(InteractionContext ctx)
    {
        await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent(TeamCreator.Instance.RestartLobby()));
    }

    [SlashCommand("print-lobby", "Prints the current members of the lobby")]
    public async Task PrintLobby(InteractionContext ctx)
    {
        await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent(TeamCreator.Instance.ShowLobby()));
    }

    [SlashCommand("join-lobby", "Joins lobby")]
    public async Task JoinLobby(InteractionContext ctx, [Option("Rank", "Type in your rank")] Rank rank,
        [Option("FirstRole", "Select your first role")]
        Role role1,
        [Option("SecondRole", "Select your second role")]
        Role role2)
    {
        var player = new Player
        {
            Name = ctx.Interaction.User.GlobalName,
            FirstRole = role1,
            SecondRole = role2,
            Rank = rank,
        };
        await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent(TeamCreator.Instance.AddPlayerToList(player)));
    }

    [SlashCommand("remove-player", "Removes player from lobby")]
    public async Task RemovePlayer(InteractionContext ctx,
        [Option("Name", "Type in the name of the person to be removed")]
        string name)
    {
        var removed = TeamCreator.Instance.RemovePlayerFromList(name);

        if (removed)
        {
            await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent("Player " + name + " removed!"));
        }
        else
        {
            await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent("Player " + name + " not found or no lobby running!"));   
        }
    }
    
    [SlashCommand("roll-teams", "Rolls Teams!")]
    public async Task RollTeams(InteractionContext ctx)
    {
        await ctx.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent(TeamCreator.Instance.Roll()));   
    }
}