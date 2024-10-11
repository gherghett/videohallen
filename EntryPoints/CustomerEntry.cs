using VideoHallen;
using VideoHallen.Models;
using VideoHallen.Services;
using VideoHallen.Exceptions;
using InputHandler;
namespace VideoHallen.EntryPoints;
public class CustomerEntry
{
    private RentingService _rentingService;
    private CustomerService _customerService;
    private InventoryService _inventoryService;

    public CustomerEntry(RentingService rentingService, 
        CustomerService customerService,
        InventoryService inventoryService)
    {
        _rentingService = rentingService;
        _customerService = customerService;
        _inventoryService = inventoryService;
    }
    
    public void AddCustomer()
    {
        try 
        {
            string name = UserGet.GetString("Input new customer name");
            string telephone = UserGet.GetString("Input telepone number");
            var customer = _customerService.AddCustomer(name, telephone);
            Console.WriteLine(customer);
        }
        catch (VideoException ex)
        {
            ErrorHandler.HandleException(ex);
        }
    }
    public void ViewAllCustomers()
    {
        try
        {
            var allCustomers = _customerService.GetAllCustomers();
            Console.WriteLine(string.Join("\n", allCustomers));
        }
        catch (VideoException ex)
        {
            ErrorHandler.HandleException(ex);
        }
    }

    public Customer? ChooseCustomer()
    {
        List<Customer> customers = new();
        MenuBuilder.CreateMenu("Find Customer")
            .OnEnter(PrintSearchResults)
            .AddScreen("Search", () =>
            {
                customers = _customerService.SearchByName(UserGet.GetString("Name of customer (search)"));
            })
            .AddQuit("Pick from results")
            .AddQuit("Pick from all",  () =>
            {
                customers = _customerService.GetAllCustomers();
            }) 
            .Enter();

        void PrintSearchResults()
        {
            if (customers.Count > 1)
                Console.WriteLine(string.Join("\n", customers));
        }

        // No customer 
        if (customers.Count < 1)
        {
            throw new VideoArgumentException("There are no search result to pick from, first do a search, exiting.");
        }

        return Chooser.ChooseAlternative<Customer>("Pick customer from search results", customers.Select(c => (c.ToString(), c)).ToArray());
    }
}