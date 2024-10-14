using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace VideoHallen.Models;

public class Customer : Entity
{
    public string Name { get; set;} = null!;
    public string Telephone { get; set; } = null!;
    // public decimal OutstandingFines { get; private set; } 
    public List<Rental> Rentals { get; set; } = null!;

    public DateOnly JoinDate { get; set; }

    public override string ToString()
    {
        return $"{Id, Formatting.IdPadding} - {Name, Formatting.NamePadding} - {Telephone}";
    }
}