using Microsoft.EntityFrameworkCore;
using VideoHallen.Models;
using VideoHallen.Exceptions;
// using VideoHallen;

namespace VideoHallen.Services;
 public partial class RentingService
{
    private readonly VideoHallDbContext _dbContext;
    public RentingService(VideoHallDbContext context)
    {
        _dbContext = context;
    }

    //Rental methods
    public Rental CreateRental(int customerId, List<int> copyIds, List<int> rentTimes)
    {
        if(copyIds.Count < 1)
            throw new VideoArgumentException("Select atleast one copy to rent");

        var customer = _dbContext.Customers.Find(customerId);
        
        if (customer == null)
            throw new CustomerNotFoundException("Customer not found");
        // TODO We need to check for customer having oustanding fees here aswell

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
            RentedCopies = copies,
            RentalTimes = rentTimes.Select(n => DateOnly.FromDateTime(DateTime.Now).AddDays(n)).ToList(),
            TimeStamp = DateTime.Now
        };

        rental.RentalPrices = CalculatePrices(rental);
        _dbContext.Add(rental);
        // _dbContext.SaveChanges();

        // Set a redundant flag, to make searching easier
        copies.ForEach(c => c.Out = true);

        _dbContext.SaveChanges();
        return rental;
    }

    public Return CreateReturn(int rentalId, List<int> copyIds)
    {
        var rental = _dbContext.Rentals.Find(rentalId);
        if (rental == null)
            throw new RentalNotFoundException("Rental not found");

        if(IsCompleted(rental))
        {
            rental.Complete = true;
            throw new VideoArgumentException("Rental is already fully returned");
        }

        if( copyIds.Count < 1)
            throw new VideoArgumentException("You have to return something to amke a return");

        var copies = _dbContext.Copies.Where(c => copyIds.Contains(c.Id)).ToList();
        if (copies.Count != copyIds.Count)
            throw new CopyNotFoundException("One or more copies not found");

        var newReturn = new Return
        {
            Rental = rental,
            ReturnedCopies = copies,
            TimeStamp = DateTime.Now
        };


        copies.ForEach(c => {
            c.Out = false;
        });

        _dbContext.Add(newReturn);

        if(IsCompleted(rental))
        {
            rental.Complete = true;
        }

        _dbContext.SaveChanges();

        List<(Copy, decimal)> fines = GetFines(rental, copies);
        fines.ForEach( f =>
        {
            if(f.Item2 > 0m)
                CreateFine(newReturn, f.Item1, rental.CustomerId, f.Item2);
        });

        return newReturn;
    }

    private Fine CreateFine(Return returnItem, Copy copy, int customer, decimal amount)
    {
        var fine = new Fine{
            Amount = amount,
            Copy = copy,
            CustomerId = customer,
            Return = returnItem
        };
        _dbContext.Add(fine);
        _dbContext.SaveChanges();
        return fine;
    }

    //Getters
    public List<Rental> GetAllRentals() => 
        _dbContext.Rentals
            .Include(r => r.Customer)
            .Include(r => r.RentedCopies)
                .ThenInclude(c => c.Rentable)
            .Include(r => r.Returns)
            .ToList();

    public Rental? GetRental(int id) => 
        _dbContext.Rentals
            .Include(r => r.Customer)
            .Include(r => r.RentedCopies)
                .ThenInclude(c => c.Rentable)
            .Include(r => r.Returns)
            .FirstOrDefault(r => r.Id == id);

    private List<(Copy, decimal)> GetFines(Rental rental, List<Copy> copies)
    {
        var results = new List<(Copy, decimal)>();
        var expireDates = GetExpireDates(rental);
        var relvantExpire = expireDates.Where(x => copies.Select(c => c.Id).Contains(x.CopyItem.Id));
        var today = DateOnly.FromDateTime(DateTime.Now);
        foreach(var x in relvantExpire)
        {
            if(x.Date <= today)
                results.Add((x.CopyItem, 0));
            else
                results.Add((x.CopyItem, DateDifference(today, x.Date) * 10));
        }
        return results;
    }
    public Return? GetReturn(int id)
    {
        return _dbContext.Returns
            .Include(r => r.Rental)
            .Include(r => r.ReturnedCopies)
            .FirstOrDefault(r => r.Id == id);
    }

    internal List<Rental> GetRentalByCustomer(Customer? customer)
    {
        if(customer is null)
            throw new VideoArgumentException("Cannot get Rentals for null customer");

        return _dbContext.Rentals
            .Include(r => r.Customer)
            .Include(r => r.RentedCopies)
                .ThenInclude(c => c.Rentable)
            .Include(r => r.Returns)
            .Where(r => r.CustomerId == customer.Id)
            .ToList();
    }

}