namespace VideoHallen.Models;

public abstract class Rentable
{
    public int Id { get; set; }
    public List<Copy> Copies { get; set; } = null!;
}

public class Movie : Rentable
{
    public string Title { get; set; } = null!;
    public List<MovieGenre> Genres{ get; set; } = null!;
    public DateOnly ReleaseDate { get; set; } 
    public override string ToString()
    {
        return $"{Id, Formatting.IdPadding} - { 
        "Movie", Formatting.RentableTypePadding} - { 
        Title, Formatting.NamePadding} { 
        ReleaseDate} { 
        //string.Join(", ", Genres.Take(5).Select(g=>g.Name))} { 
        (Copies != null ? "Copies: "+Copies.Count : "")} { 
        ""}";
    }
}

public class Game : Rentable
{
    public string Title {get; set;} = null!;
    public GamePublisher Publisher { get; set; } = null!;
    public DateOnly ReleaseDate { get; set; }
    public override string ToString()
    {
        return $"{Id, Formatting.IdPadding} - { 
        "Game", Formatting.RentableTypePadding} - { 
        Title, Formatting.NamePadding} { 
        ReleaseDate} { 
        Publisher} { 
        (Copies != null ? "Copies: "+Copies.Count : "")} { 
        ""}";
    }
}

public class RentConsole : Rentable
{
    public string Model { get; set; } = null!;
    public override string ToString()
    {
        return $"{Id, Formatting.IdPadding} - { 
        "Console", Formatting.RentableTypePadding} - { 
        Model, Formatting.NamePadding} { 
        //(Copies != null ? "Copies: "+Copies.Count : "")} { 
        ""}";
    }

}