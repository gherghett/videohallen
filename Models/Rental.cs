using System.Text;

namespace VideoHallen.Models;

public class Rental
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer{ get; set; } = null!;
    public DateTime TimeStamp { get; set; }
    public List<Copy> RentedCopies { get; set; } = null!;
    public List<DateOnly> RentalTimes { get; set;} = null!;
    public List<decimal> RentalPrices { get; set;} = null!;
    public List<Return> Returns { get; set; } = null!;
    public bool Complete { get; set; } = false;

    public override string ToString() =>
        $"{Id}:{Customer.Name[..Math.Min(10, Customer.Name.Length)]}, {TimeStamp}, {RentedCopies.Count} copies, all returned: {Complete}";

    public string ToStringLong()
    {
        return $"Rental Id: {Id,Formatting.IdPadding} - {Customer.Name,Formatting.NamePadding} - Date: {TimeStamp} - Copies: {RentedCopies.Count}, Returned: {Complete}";
    }
}