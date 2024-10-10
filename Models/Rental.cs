namespace VideoHallen.Models;

public class Rental
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer{ get; set; } = null!;
    public DateTime TimeStamp { get; set; }
    public List<Copy> RentedCopies { get; set; } = null!;
    public List<Return> Returns { get; set; } = null!;
    public bool Complete { get; set; } = false;
}