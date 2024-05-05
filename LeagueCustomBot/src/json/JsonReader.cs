using Newtonsoft.Json;

namespace LeagueCustomBot.json;

internal class JsonReader
{
    public string Token { get; set; } = "";
    public string Prefix { get; set; } = "";
    
    public async Task ReadJson()
    {
        using var streamReader = new StreamReader(@"C:\Users\japog\RiderProjects\LeagueCustomBot\LeagueCustomBot\config\config.json");
        
        var json = await streamReader.ReadToEndAsync();
        var data = JsonConvert.DeserializeObject<JsonStructure>(json);

        if (data != null)
        {
            this.Token = data.Token;
            this.Prefix = data.Prefix;    
        }
    }
}

internal sealed class JsonStructure
{
    public string Token { get; set; } = "";
    public string Prefix { get; set; } = "";
}