namespace VideoHallen.Models;

public abstract class Rentable
{
    public abstract string Name();
    public int Id { get; set; }
    public List<Copy> Copies { get; set; } = null!;

    public override string ToString()
    {
        return $"{Id, Formatting.IdPadding} - { 
        this.GetType().Name, Formatting.RentableTypePadding} - { 
        Name(), Formatting.NamePadding} { 
        (Copies != null ? "Copies: "+Copies.Count : "")} { 
        ""}";
    }
}

public class Movie : Rentable
{

    public string Title { get; set; } = null!;
    public List<MovieGenre> Genres{ get; set; } = null!;
    public DateOnly ReleaseDate { get; set; } 
    public override string ToString()
    {
        return $"{base.ToString()} {  
        ReleaseDate} { 
        //string.Join(", ", Genres.Take(5).Select(g=>g.Name))} { 
        ""}";
    }
    public override string Name()
    {
        return Title;
    }
}

public class Game : Rentable
{
    public string Title {get; set;} = null!;
    public GamePublisher Publisher { get; set; } = null!;
    public DateOnly ReleaseDate { get; set; }
    public override string ToString()
    {
        return $"{base.ToString()} {  
        ReleaseDate} { 
        Publisher} { 
        ""}";
    }
    public override string Name()
    {
        return Title;
    }
}

public class RentConsole : Rentable
{
    public string Model { get; set; } = null!;
    public override string ToString()
    {
        return $"{base.ToString()} {  
        //(Copies != null ? "Copies: "+Copies.Count : "")} { 
        ""}";
    }
    public override string Name()
    {
        return Model;
    }

}