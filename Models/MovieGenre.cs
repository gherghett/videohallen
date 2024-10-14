namespace VideoHallen.Models;

public class MovieGenre  : Entity
{
    public string Name { get; set; } = null!;
    public List<Movie> Movies{ get; set; } = null!;
}