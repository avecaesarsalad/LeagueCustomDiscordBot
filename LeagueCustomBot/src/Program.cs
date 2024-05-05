using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using LeagueCustomBot.commands;
using LeagueCustomBot.json;
using LeagueCustomBot.teamcreator;

namespace LeagueCustomBot;

internal static class Program
{
    private static DiscordClient Client { get; set; } = null!;
    private static CommandsNextExtension Commands { get; set; } = null!;

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
        
        var slashCommandConfiguration = Client.UseSlashCommands();
        
        slashCommandConfiguration.RegisterCommands<BasicCommands>();
        

        await Client.ConnectAsync();
        await Task.Delay(-1);
    }
}