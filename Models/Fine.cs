namespace VideoHallen.Models;
public class Fine : Entity
{
    public string Reason { get; set; } = null!;
    public int RentedCopyId { get; set; }
    public RentedCopy RentedCopy { get; set; } = null!;
    public int CustomerId { get; set; } 
    public Customer Customer { get; set; } = null!;
    public decimal Amount {get; set;} 
    public bool Paid {get; set;} = false;
    // public DateTime timeStamp {get; set;} 
}