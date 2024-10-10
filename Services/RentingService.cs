using Microsoft.EntityFrameworkCore;
using VideoHallen;
using VideoHallen.Models;

namespace VideoHallen.Services;
public class RentingService 
{
    private readonly VideoHallDbContext _dbContext;
    public RentingService(VideoHallDbContext context)
    {
        _dbContext = context;
    }

    //Rental methods
    public int CreateRental(int customerId, List<int> copyIds)
    {
        var customer = _dbContext.Customers.Find(customerId);
        if (customer == null)
            throw new ArgumentException("Customer not found");

        var copies = _dbContext.Copies.Where(c => copyIds.Contains(c.Id)).ToList();
        if (copies.Count != copyIds.Count)
            throw new ArgumentException("One or more copies not found");

        var rental = new Rental
        {
            Customer = customer,
            RentedCopies = copies,
            TimeStamp = DateTime.Now
        };

        _dbContext.Add(rental);
        _dbContext.SaveChanges();
        return rental.Id;
    }

    public Rental? GetRental(int id)
    {
        return _dbContext.Rentals
            .Include(r => r.Customer)
            .Include(r => r.RentedCopies)
                .ThenInclude(c => c.Rentable)
            .Include(r => r.Returns)
            .FirstOrDefault(r => r.Id == id);
    }

    // Return methods
    public int CreateReturn(int rentalId, List<int> copyIds)
    {
        var rental = _dbContext.Rentals.Find(rentalId);
        if (rental == null)
            throw new ArgumentException("Rental not found");

        var copies = _dbContext.Copies.Where(c => copyIds.Contains(c.Id)).ToList();
        if (copies.Count != copyIds.Count)
            throw new ArgumentException("One or more copies not found");

        var newReturn = new Return
        {
            Rental = rental,
            ReturnedCopies = copies,
            TimeStamp = DateTime.Now
        };

        _dbContext.Add(newReturn);
        _dbContext.SaveChanges();
        return newReturn.Id;
    }

    public Return? GetReturn(int id)
    {
        return _dbContext.Returns
            .Include(r => r.Rental)
            .Include(r => r.ReturnedCopies)
            .FirstOrDefault(r => r.Id == id);
    }

}