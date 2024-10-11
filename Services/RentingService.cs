
using Microsoft.EntityFrameworkCore;
using VideoHallen.Models;
using VideoHallen.Exceptions;
// using VideoHallen;

namespace VideoHallen.Services;
public partial class RentingService
{
    // private readonly VideoHallDbContext _dbContext;
    // // public RentingService(VideoHallDbContext context)
    // // {
    // //     _dbContext = context;
    // // }

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

    //service
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

    private int DateDifference(DateOnly date1, DateOnly date2)
    {
        return (date1.ToDateTime(TimeOnly.MinValue) - date2.ToDateTime(TimeOnly.MinValue)).Days;
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