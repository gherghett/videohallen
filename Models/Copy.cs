namespace VideoHallen.Models;
public class Copy
{
    public int Id { get; set; }
    public int RentableId { get; set; }
    public Rentable Rentable { get; set; } = null!;
    public List<Rental> Rentals{ get; set; } = null!;
    public List<Return> Returns{ get; set; } = null!;
    public bool Damaged = false;
    public bool Unusable = false;
    public bool Out = false;
}