namespace VideoHallen.Models;

public class Return
{
    public int Id { get; set; }
    public DateTime TimeStamp { get; set; }
    public int RentalId { get; set; }
    public Rental Rental { get; set; } = null!;
    public List<Copy> ReturnedCopies { get; set; } = null!;
}