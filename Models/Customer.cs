using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace VideoHallen.Models;

public class Customer
{
    public int Id { get; set;}
    public string Name { get; set;} = null!;
    public List<Rental> Rentals { get; set; } = null!;

    public override string ToString()
    {
        return $"{Id, Formatting.IdPadding} - {Name, Formatting.NamePadding}x";
    }
}