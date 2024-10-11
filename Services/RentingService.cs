using Microsoft.EntityFrameworkCore;
using VideoHallen.Models;
using VideoHallen.Exceptions;
// using VideoHallen;

namespace VideoHallen.Services;
public class RentingService
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

    private List<decimal> CalculatePrices(Rental rental)
    {
        var prices = new List<decimal>();
        var startTime = DateOnly.FromDateTime(rental.TimeStamp);
        foreach(var copy in GetExpireDates(rental))
        {
            int days = DateDifference(copy.Date, startTime);
            
            prices.Add(CalculatePrice(copy.CopyItem.Rentable, days));
        }
        return prices;
    }

    private int DateDifference(DateOnly date1, DateOnly date2)
    {
        return (date1.ToDateTime(TimeOnly.MinValue) - date2.ToDateTime(TimeOnly.MinValue)).Days;
    }

    decimal CalculatePrice(Rentable rentable, int days)
    {
        if( rentable is RentConsole)
        {
            if (days == 7)
            {
                return 349m;
            }
            else 
            {
                return 99m * days;
            }
        }
        else
        {
            return  29m * days;
        }
    }

    private List<( Copy CopyItem, DateOnly Date ) > GetExpireDates(Rental rental)
    {
        // var loadedRental = _dbContext.Rentals
        //     .Include(r => r.RentedCopies)
        //         .ThenInclude(c => c.Rentable)
        //     .Where(r => r.Id == rental.Id)
        //     .SingleOrDefault();
        // if (loadedRental == null)
        //      throw new RentalNotFoundException("Could not load Rental");

        return rental.RentedCopies.Zip(rental.RentalTimes, (x, y) => (CopyItem: x ,Date: y)).ToList();
    }



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

    // Return methods
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
                AddFine(newReturn, f.Item1, rental.CustomerId, f.Item2);
        });

        return newReturn;
    }

    private Fine AddFine(Return returnItem, Copy copy, int customer, decimal amount)
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

    public bool IsCompleted(Rental rental)
    {
        var loadedRental = _dbContext.Rentals
            .Include(r => r.RentedCopies)
            .Where(r => r.Id == rental.Id)
            .SingleOrDefault();

        if (loadedRental == null)
            throw new RentalNotFoundException("Could not access rental.");

        return loadedRental.RentedCopies.All(c => IsReturned(loadedRental, c));
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

    public bool IsReturned(Rental rental, Copy copy)
    {
        var loadedRental = _dbContext.Rentals
            .Include(r => r.Returns)
                .ThenInclude(r => r.ReturnedCopies)
            .FirstOrDefault(r => r.Id == rental.Id);
        if(loadedRental is null)
            throw new RentalNotFoundException("Rental requested was not found in database", rental.Id);
        
        return loadedRental
                .Returns
                .Any(r => r.ReturnedCopies.Any( c => c.Id == copy.Id));
    }
}