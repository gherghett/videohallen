using System.Text;

namespace VideoHallen.Models;

public class Rental  : Entity
{
    public int CustomerId { get; set; }
    public Customer Customer{ get; set; } = null!;
    public DateTime TimeStamp { get; set; }
    public List<RentedCopy> RentedCopys { get; set; } = null!;
    //public List<DateOnly> RentalTimes { get; set;} = null!;
    //public List<decimal> RentalPrices { get; set;} = null!;
    //public List<Return> Returns { get; set; } = null!;
    public bool Complete { get; set; } = false;
    public decimal Price {get; set;}

    public override string ToString() =>
        $"{Id}:{Customer.Name[..Math.Min(10, Customer.Name.Length)]}, {TimeStamp}, {RentedCopys.Count} copies, all returned: {Complete}";

    public string ToStringLong()
    {
        return $"Rental Id: {Id,Formatting.IdPadding} - {Customer.Name,Formatting.NamePadding} - Date: {TimeStamp} - Copies: {RentedCopys.Count}, Returned: {Complete}";
    }
}