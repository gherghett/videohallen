namespace VideoHallen.Models;

public class GamePublisher
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<Game> Games{ get; set; } = null!;

    public override string ToString() =>
        $"{Id, Formatting.IdPadding} - {Name, Formatting.NamePadding}";
}