namespace VideoHallen.Models;
public class Fine
{
    public int Id { get; set; }
    public int ReturnId { get; set; }
    public Return Return { get; set; } = null!;
    public int CopyId { get; set; }
    public Copy Copy { get; set; } = null!;
    public int CustomerId { get; set; } 
    public Customer Customer { get; set; } = null!;
    public decimal Amount {get; set;} 
}