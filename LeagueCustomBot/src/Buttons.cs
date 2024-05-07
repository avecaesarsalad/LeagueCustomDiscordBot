using DSharpPlus.Entities;

namespace LeagueCustomBot;

public static class Buttons
{
    public static DiscordButtonComponent StartLobbyButton => new(DiscordButtonStyle.Primary, "start", "Start Lobby");
    public static DiscordButtonComponent RollTeamsButton => new(DiscordButtonStyle.Primary, "roll", "Roll Lobby");
    public static DiscordButtonComponent RerollTeamsButton => new(DiscordButtonStyle.Primary, "reroll", "Reroll Lobby");
    public static DiscordButtonComponent MoveTeamsButton => new(DiscordButtonStyle.Primary, "move", "Move Teams");
    public static DiscordButtonComponent MoveTeamsBackButton => new(DiscordButtonStyle.Primary, "moveback", "Move Teams back");
}