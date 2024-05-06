using DSharpPlus.Entities;

namespace LeagueCustomBot;

public static class Functions
{
    public static async Task DeleteLastMessages(DiscordInteraction discordInteraction)
    {
        var channel = discordInteraction.Channel;
        var messages = channel.GetMessagesAsync(3);
        
        await foreach(var message in messages)
        {
            await message.DeleteAsync();
        }
    }
}