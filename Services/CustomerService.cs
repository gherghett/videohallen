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

    public Customer AddCustomer(string name)
    {
        Customer newCustomer = new Customer{Name = name};
        _dbContext.Add(newCustomer);
        _dbContext.SaveChanges();
        return newCustomer;
    }

    public List<Customer> GetAllCustomers()
    {
        return _dbContext.Customers.ToList();
    }

}
