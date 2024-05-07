namespace LeagueCustomBot;

public record Player
{
    public ulong Id { get; set; }
    public string Name { get; set; } = "";
    public Rank Rank { get; set; }
    public Role FirstRole { get; set; }
    public Role SecondRole { get; set; }
    public Role SelectedRole { get; set; }
}