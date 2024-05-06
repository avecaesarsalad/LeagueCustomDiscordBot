using DSharpPlus.Entities;
using LeagueCustomBot.teamcreator;

namespace LeagueCustomBot;

public static class Buttons
{
    public static DiscordButtonComponent StartLobbyButton => new DiscordButtonComponent(DiscordButtonStyle.Primary, "start", "Start Lobby");
    public static DiscordButtonComponent RollTeamsButton => new DiscordButtonComponent(DiscordButtonStyle.Primary, "roll", "Roll Lobby");
    public static DiscordButtonComponent RerollTeamsButton => new DiscordButtonComponent(DiscordButtonStyle.Primary, "roll", "Reroll Lobby");
}