namespace VideoHallen.Models;

public class MovieGenre 
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public List<Movie> Movies{ get; set; } = null!;
}