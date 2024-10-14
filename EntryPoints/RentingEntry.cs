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

            HashSet<Copy> copies = new(); //we dont want to rent the same copy two times at once
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
                    rentinglengthOptions.Add(7); // Här gömmer sig lite business logic i fel lager

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
            // menu for choosin what Rental to return copies from
            var rental = ChooseRental();

            // MEnu to choose what copies in the rental to return
            var copiesToReturn = rental.RentedCopys.Where(rc => !_rentingService.IsReturned(rc));
            var returnedCopies = rental.RentedCopys.Where(rc => _rentingService.IsReturned(rc));
            var choiceOfReturn = new List<int>();
            if (copiesToReturn.Any())
            {
                choiceOfReturn =
                Chooser.ChooseMultiple<int>("What do you want to return?",
                    copiesToReturn.Select(rc => (_inventoryService.GetCopy(rc.CopyId).ToString(), rc.CopyId)).ToArray());
            }

            var returnResult = _rentingService.MakeReturn(rental.Id, choiceOfReturn);

            // Mark any damaged or destroyed 
            var options = returnResult.Select(rc => (rc.Copy.ToString(), rc)).ToArray();
            var damaged = Chooser.ChooseMultiple<RentedCopy>("Was anything damaged (not destroyed)?", options);
            var destroyed = Chooser.ChooseMultiple<RentedCopy>("Was anything Destroyed?", options);
            damaged.ForEach(d => _inventoryService.SetDamaged(d));
            destroyed.ForEach(d => _inventoryService.SetDestroyed(d));

            // show fines for customer
            var fines = _customerService.GetFinesForCustomer(rental.CustomerId);
            if (fines.Count > 0)
            {
                Console.WriteLine("Added fines:");
                foreach (var fine in fines.Where(f => returnResult.Select(r => r.Id).Contains(f.RentedCopyId)))
                {
                    Console.WriteLine($"id: {fine.Id}, {fine.Amount} for {fine.Reason} concerning RentedCopy: {fine.RentedCopyId},");
                }
            }
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
            throw new VideoArgumentException("There are no rentals to choose from");
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
        foreach (var rentedCopy in loadedRental.RentedCopys)
        {
            sb.AppendLine("-\tRental: (" + _inventoryService.GetRentableOfCopy(rentedCopy.CopyId) + ")");
            sb.AppendLine($"\tCopy id: {rentedCopy.CopyId} returned: {_rentingService.IsReturned(rentedCopy)}");
            sb.AppendLine("\treturn by: " + rentedCopy.DueByDate);
            sb.AppendLine("\tPrice: " + rentedCopy.Price);
        }
        sb.AppendLine("-------------------");
        sb.AppendLine("Total Price: " + loadedRental.Price);

        return sb.ToString();
    }

}