using Microsoft.EntityFrameworkCore;
using VideoHallen;
using VideoHallen.Models;

namespace VideoHallen.Services;
public class CustomerService 
{
    private readonly VideoHallDbContext _dbContext;
    public CustomerService(VideoHallDbContext context)
    {
        _dbContext = context;
    }

    public Customer AddCustomer(string name, string telephone)
    {
        Customer newCustomer = new Customer{Name = name, Telephone = telephone};
        _dbContext.Add(newCustomer);
        _dbContext.SaveChanges();
        return newCustomer;
    }

    public List<Customer> GetAllCustomers()
    {
        return _dbContext.Customers.ToList();
    }

    public List<Customer> SearchByName(string query)
    {
        return _dbContext.Customers
            .Where(c => EF.Functions.Like(c.Name, $"%{query}%"))
            .ToList();
    }

}
