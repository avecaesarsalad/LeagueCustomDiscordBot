using System.Reflection;
using Newtonsoft.Json;

namespace LeagueCustomBot.json;

internal class JsonReader
{
    public string Token { get; set; } = "";
    public string Prefix { get; set; } = "";
    public ulong? RedTeamChannelId { get; set; }
    public ulong? BlueTeamChannelId { get; set; }
    public ulong? BaseChannelId { get; set; }
    
    public async Task ReadJson(JsonTypes jsonType)
    {
        switch (jsonType)
        {
            case JsonTypes.Config:
                var configReader = new StreamReader("../../../config/config.json");
        
                var jsonConfig = await configReader.ReadToEndAsync();
                var dataConfig = JsonConvert.DeserializeObject<JsonStructureConfig>(jsonConfig);

                if (dataConfig != null)
                {
                    this.Token = dataConfig.Token;
                    this.Prefix = dataConfig.Prefix;
                }
                break;
            
            
            case JsonTypes.Channels:
                var channelsReader = new StreamReader("../../../config/channels.json");
        
                var jsonChannels = await channelsReader.ReadToEndAsync();
                var dataChannels = JsonConvert.DeserializeObject<JsonStructureChannels>(jsonChannels);

                if (dataChannels != null)
                {
                    this.RedTeamChannelId = dataChannels.RedTeamChannelId;
                    this.BlueTeamChannelId = dataChannels.BlueTeamChannelId;
                    this.BaseChannelId = dataChannels.BaseChannelId;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(jsonType), jsonType, null);
        }
    }
    
    public async Task WriteToJson(ulong redTeamChannelId, ulong blueTeamChannelId, ulong baseChannelId)
    {
        var data = new JsonStructureChannels()
        {
            RedTeamChannelId = redTeamChannelId,
            BlueTeamChannelId = blueTeamChannelId,
            BaseChannelId = baseChannelId,
        };

        var json = JsonConvert.SerializeObject(data);

        await using var streamWriter = new StreamWriter("../../../config/channels.json");
        await streamWriter.WriteAsync(json);
    }
}

internal sealed class JsonStructureConfig
{
    public string Token { get; set; } = "";
    public string Prefix { get; set; } = "";
}

internal sealed class JsonStructureChannels
{
    public ulong? RedTeamChannelId { get; set; }
    public ulong? BlueTeamChannelId { get; set; }
    public ulong? BaseChannelId { get; set; }
}