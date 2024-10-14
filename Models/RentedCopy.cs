using System.Text;

namespace VideoHallen.Models;

public class RentedCopy : Entity
{
    public int RentalId { get; set; }
    public Rental Rental { get; set; } = null!;
    public int CopyId { get; set; }
    public Copy Copy { get; set; } = null!;
    public DateTime? ReturnDate { get; set; } = null!;
    public DateOnly DueByDate {get; set;} 

    public List<Fine> Fines { get; set; } = null!;
    public decimal Price {get; set;}
}