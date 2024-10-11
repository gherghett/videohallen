using VideoHallen;
using VideoHallen.Models;
using VideoHallen.Services;
using VideoHallen.Exceptions;
using InputHandler;
using System.Text;

namespace VideoHallen.EntryPoints;
public class RentingEntry
{
    private InventoryEntry _inventoryEntry;
    private CustomerEntry _customerEntry;
    private RentingService _rentingService;
    private CustomerService _customerService;
    private InventoryService _inventoryService;

    public RentingEntry(
        InventoryEntry inventoryEntry,
        CustomerEntry customerEntry,
        RentingService rentingService,
        CustomerService customerService,
        InventoryService inventoryService)
    {
        _inventoryEntry = inventoryEntry;
        _customerEntry = customerEntry;

        _rentingService = rentingService;
        _customerService = customerService;
        _inventoryService = inventoryService;
    }
    public void NewRental()
    {
        try
        {
            var customer = _customerEntry.ChooseCustomer();

            // if (customer is null)
            //     return;

            HashSet<Copy> copies = new(); //we dont want to rent the same two times at once
            MenuBuilder.CreateMenu("Add items to be rented")
                .OnEnter(PrintAddedCopies)
                .AddScreen("Add copy", () =>
                {
                    copies.Add(_inventoryEntry.ChooseCopy());
                })
                .AddQuit("Continue")
                .Enter();

            void PrintAddedCopies() => 
                Console.WriteLine(
                    copies.Count > 0
                        ? "Copies added so far: " + string.Join("\n", copies)
                        : "No copies added yet.");

            List<int> rentTimes = new();
            foreach (var copy in copies)
            {
                List<int> rentinglengthOptions = [1, 2, 3];
                if (copy.Rentable is RentConsole)
                    rentinglengthOptions.Add(7);

                rentTimes.Add(Chooser.ChooseAlternative<int>($"How long is rent on {copy.Rentable.Name()}?",
                     rentinglengthOptions.Select(n => (n.ToString(), n)).ToArray()));
            }

            Rental rental = _rentingService.CreateRental(
                customer.Id,
                copies.Select(c => c.Id).ToList(),
                rentTimes
            );

            Console.WriteLine(RentalReceiptString(rental));

        }
        catch (VideoException ex)
        {
            ErrorHandler.HandleException(ex);
        }
    }

    public void ViewAllRentals()
    {
        try
        {
            List<Rental> rentals = _rentingService.GetAllRentals();
            Console.WriteLine(string.Join("\n", rentals));
        }
        catch (VideoException ex)
        {
            ErrorHandler.HandleException(ex);
        }
    }

    internal void InspectRental()
    {
        try
        {
            Rental? rental = ChooseRental();
            if (rental == null)
            {
                return;
            }
            Console.WriteLine(RentalReceiptString(rental));
        }
        catch (VideoException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void NewReturn()
    {
        try
        {
            var rental = ChooseRental();
            var copiesToReturn = rental.RentedCopies.Where(c => !_rentingService.IsReturned(rental, c));
            var returnedCopies = rental.RentedCopies.Where(c => _rentingService.IsReturned(rental, c));

            var choiceOfReturn = new List<int>();
            if (copiesToReturn.Any())
            {
                choiceOfReturn =
                Chooser.ChooseMultiple<int>("What do you want to return?",
                    copiesToReturn.Select(c => (c.ToString(), c.Id)).ToArray());
            }
            var returnResult = _rentingService.CreateReturn(rental.Id, choiceOfReturn);
            Console.WriteLine("you returned: \n " + RentalReceiptString(returnResult.Rental));
        }
        catch (VideoException ex)
        {
            ErrorHandler.HandleException(ex);
        }

    }

    private Rental ChooseRental()
    {
        List<Rental> rentals = new();
        MenuBuilder.CreateMenu("Choose Rental")
            .AddQuit("Rental by customer", () =>
            {
                rentals = _rentingService.GetRentalByCustomer(_customerEntry.ChooseCustomer());
            })
            .AddQuit("Pick from all", () =>
            {
                rentals = _rentingService.GetAllRentals();
            })
            .Enter();


        // No customer 
        if (rentals.Count < 1)
        {
            throw new VideoArgumentException("There are no search result to pick from, first do a search, exiting.");
        }

        return Chooser.ChooseAlternative<Rental>("Choose rental", rentals.Select(r => (r.ToString(), r)).ToArray());
    }

    public string RentalReceiptString(Rental rental)
    {
        var loadedRental = _rentingService.GetRental(rental.Id);
        if (loadedRental == null)
            throw new RentalNotFoundException("Could not load rental");

        var sb = new StringBuilder("-------------------\n", 20000);
        sb.AppendLine(loadedRental.ToStringLong() + "\n");
        sb.AppendLine("Copies: ");
        int index = 0;
        foreach (var copy in loadedRental.RentedCopies)
        {
            sb.AppendLine("\t" + copy + $" returned: {_rentingService.IsReturned(rental, copy)}");
            sb.AppendLine("\treturn by: " + loadedRental.RentalTimes[index]);
            sb.AppendLine("\tPrice: " + loadedRental.RentalPrices[index++]);
        }
        sb.AppendLine("-------------------");
        sb.AppendLine("Total Price: "+loadedRental.RentalPrices.Sum());

        return sb.ToString();
    }

}