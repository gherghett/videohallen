
using Microsoft.EntityFrameworkCore;
using VideoHallen.Models;
using VideoHallen.Exceptions;
// using VideoHallen;

namespace VideoHallen.Services;
public partial class RentingService
{
    private int DateDifference(DateOnly date1, DateOnly date2) =>
        (date1.ToDateTime(TimeOnly.MinValue) - date2.ToDateTime(TimeOnly.MinValue)).Days;

    public bool IsCompleted(Rental rental)
    {
        var loadedRental = _dbContext.Rentals
            .Include(r => r.RentedCopys)
                .ThenInclude(rc => rc.Copy)
            .SingleOrDefault(r => r.Id == rental.Id);

        if (loadedRental == null)
            throw new RentalNotFoundException("Could not access rental.");

        return loadedRental.RentedCopys.All(c => IsReturned(loadedRental, c.Copy));
    }

    public bool IsReturned(Rental rental, Copy copy)
    {
        var loadedRental = _dbContext.Rentals
            .Include(r => r.RentedCopys)
                .ThenInclude(rc => rc.Copy)
            .SingleOrDefault(r => r.Id == rental.Id);
        if(loadedRental is null)
            throw new RentalNotFoundException("Rental requested was not found in database", rental.Id);
        
        return loadedRental
                .RentedCopys
                .Any(r => r.CopyId == copy.Id && r.ReturnDate != null);
    }

    public bool IsReturned(RentedCopy rentedCopy)
    {
        // var loadedRentedCopy = _dbContext.RentedCopys
        //     .Include(r => r.Copy)
        //     .SingleOrDefault(r => r.Id == rentedCopy.Id);
            
        // if(loadedRentedCopy is null)
        //     throw new RentalNotFoundException("RentalCopy requested was not found in database", rentedCopy.Id);
        
        return rentedCopy.ReturnDate != null;
    }


    private decimal CalculateFine(int daysLate)
    {
        return 10m * daysLate; 
    }

    public void CreateFine(RentedCopy rc, decimal amount, string reason)
    {
        var rental = GetRental(rc.RentalId);
        if(rental is null)
            throw new RentalNotFoundException("Rental could not be loaded from db");

        var fine = new Fine {
            Amount = amount,
            Customer = rental.Customer,
            RentedCopy = rc,
            Reason = reason
        };
        _dbContext.Add(fine);
    }
}