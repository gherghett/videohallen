namespace VideoHallen.Models;
public class Copy
{
    public int Id { get; set; }
    public int RentableId { get; set; }
    public Rentable Rentable { get; set; } = null!;
    public List<Rental> Rentals{ get; set; } = null!;
    public List<Return> Returns{ get; set; } = null!;
    public bool Damaged {get; set;} = false;
    public bool Unusable {get; set;} = false;
    public bool Out { get; set; } = false;

    public override string ToString()
    {
        return $"{Id} {Rentable?.GetType().Name} {Rentable?.Name()} Out: {Out}";
    }
}