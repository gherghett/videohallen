using Microsoft.EntityFrameworkCore;
using VideoHallen.Models;
using VideoHallen.Exceptions;
// using VideoHallen;
using VideoHallen.Services.Pricing;


namespace VideoHallen.Services;
public partial class RentingService
{
    private readonly VideoHallDbContext _dbContext;
    private readonly IPricingStrategy _pricingStrategy;
    private readonly ITotalPriceStrategy _totalPriceStrategy;
    //private readonly InventoryService _inventoryService;
    public RentingService(VideoHallDbContext context,
        IPricingStrategy pricingStrategy,
        ITotalPriceStrategy totalPriceStrategy
    )
    {
        _dbContext = context;
        _pricingStrategy = pricingStrategy;
        _totalPriceStrategy = totalPriceStrategy;
        //_inventoryService = inventoryService;
    }

    //Rental methods
    public Rental CreateRental(int customerId, List<int> copyIds, List<int> rentTimes)
    {
        if (copyIds.Count < 1)
            throw new VideoArgumentException("Select atleast one copy to rent");

        var customer = _dbContext.Customers.Find(customerId);

        if (customer == null)
            throw new CustomerNotFoundException("Customer not found");
        
        var fines = GetUnpaidFinesForCustomer(customer);
        if( fines.Count > 0)
            throw new RuleBreakException("Cant make a new rental! Outstanding Fines!");
        // There is no way to pay fine

        var copies = _dbContext.Copies.Include(c => c.Rentable).Where(c => copyIds.Contains(c.Id)).ToList();

        if (copies.Count != copyIds.Count)
            throw new CopyNotFoundException("One or more copies not found",
                copyIds.Where(c => !copies.Select(co => co.Id).Contains(c)).ToList());
        if (copies.Where(c => c.Out).Any())
            throw new CopyNotAvailableException("One or more Copies are not available for rent",
                copies.Where(c => c.Out).Select(c => c.Id).ToList());

        var rental = new Rental
        {
            Customer = customer,
            //RentedCopys = 
            //RentalTimes = rentTimes.Select(n => DateOnly.FromDateTime(DateTime.Now).AddDays(n)).ToList(),
            TimeStamp = DateTime.Now
        };

        var rentedCopys = 
        copies
            .Zip(rentTimes, (c, t) =>
                new RentedCopy
                {
                    Copy = c,
                    DueByDate = DateOnly.FromDateTime(DateTime.Now).AddDays(t),
                    //Price = CalculatePrice(GetRentableOfCopy(c.Id), t)
                }).ToList();
        rental.RentedCopys = rentedCopys;

        _dbContext.Add(rental); 

        rentedCopys.ForEach(rc => rc.Price = _pricingStrategy.CalculatePrice(rc));

        rental.Price = _totalPriceStrategy.CalculateTotalPrice(rental);

        _dbContext.SaveChanges();

        // Set a redundant flag, to make searching easier
        copies.ForEach(c => c.Out = true);

        _dbContext.SaveChanges();
        return rental;
    }

    public List<RentedCopy> MakeReturn(int rentalId, List<int> copyIds)
    {
        var rental = _dbContext
            .Rentals
            .Include(r => r.RentedCopys)
                .ThenInclude(rc => rc.Copy)
            .Where(r => r.Id == rentalId).SingleOrDefault();
        if (rental == null)
            throw new RentalNotFoundException("Rental not found");

        if (IsCompleted(rental))
        {
            rental.Complete = true;
            throw new VideoArgumentException("Rental is already fully returned");
        }

        if (copyIds.Count < 1)
            throw new VideoArgumentException("You have to return something to amke a return");

        var copies = _dbContext.Copies.Where(c => copyIds.Contains(c.Id)).ToList();
        if (copies.Count != copyIds.Count)
            throw new CopyNotFoundException("One or more copies not found");

        // var newReturn = new Return
        // {
        //     Rental = rental,
        //     ReturnedCopies = copies,
        //     TimeStamp = DateTime.Now
        // };

        var rentedCopys = new List<RentedCopy>();
        copies.ForEach(c =>
        {
            c.Out = false;
            var rentedCopy = rental.RentedCopys
                .Where(r => r.CopyId == c.Id)
                .Single();
            rentedCopy.ReturnDate = DateTime.Now;
            // rentedCopy.DueByDate
            var daysLate = DateDifference(DateOnly.FromDateTime(DateTime.Now),rentedCopy.DueByDate); 
            if( daysLate > 0)
            {
                var fine = new Fine
                {
                    Amount = CalculateFine(daysLate),
                    CustomerId = rental.CustomerId,
                    RentedCopy = rentedCopy,
                    Reason = "Late fees"
                };
                if (rentedCopy.Fines == null)
                {
                    _dbContext.Entry(rentedCopy).Collection(r => r.Fines).Load();
                }
                if( rentedCopy == null || rentedCopy.Fines == null)
                    throw new RentalNotFoundException("Database issue");
                rentedCopy.Fines.Add(fine); ///loaded?
            }
            rentedCopys.Add(rentedCopy);
        });

        if (IsCompleted(rental))
        {
            rental.Complete = true;
        }

        _dbContext.SaveChanges();

        return rentedCopys;
    }

    //Getters
    public List<Rental> GetAllRentals() =>
        _dbContext.Rentals
            .Include(r => r.Customer)
            .Include(r => r.RentedCopys)
                .ThenInclude(rc => rc.Copy)
                    .ThenInclude(c => c.Rentable)
            .ToList();

    public Rental? GetRental(int id) =>
        _dbContext.Rentals
            .Include(r => r.Customer)
            .Include(r => r.RentedCopys)
                .ThenInclude(rc => rc.Copy)
                    .ThenInclude(c => c.Rentable)
            .FirstOrDefault(r => r.Id == id);

    internal List<Rental> GetRentalByCustomer(Customer customer)
    {
        // if (customer is null)
        //     throw new VideoArgumentException("Cannot get Rentals for null customer");

        return _dbContext.Rentals
            .Include(r => r.Customer)
            .Include(r => r.RentedCopys)
                .ThenInclude(rc => rc.Copy)
            .Where(r => r.CustomerId == customer.Id)
            .ToList();
    }

    public List<Fine> GetUnpaidFinesForCustomer(Customer customer)
    {
        return _dbContext
            .Fines
            .Include(f => f.RentedCopy)
                .ThenInclude(rc => rc.Copy)
            .Where(f => f.CustomerId == customer.Id && !f.Paid ).ToList();
        
    }

    public Rentable GetRentableOfCopy(int copyId)
    {
        var rentable = _dbContext.Rentables.Where(r => r.Copies.Any( c => c.Id == copyId)).SingleOrDefault();
        if (rentable is null)
            throw new RentalNotFoundException("Could not load that rentable....");
        return rentable;
    } 
}