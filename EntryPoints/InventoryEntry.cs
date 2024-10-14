using VideoHallen;
using VideoHallen.Models;
using VideoHallen.Services;
using InputHandler;
using VideoHallen.Exceptions;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace VideoHallen.EntryPoints;
public class InventoryEntry
{
    private RentingService _rentingService;
    private CustomerService _customerService;
    private InventoryService _inventoryService;

    public InventoryEntry(RentingService rentingService,
        CustomerService customerService,
        InventoryService inventoryService)
    {
        _rentingService = rentingService;
        _customerService = customerService;
        _inventoryService = inventoryService;
    }

    public void ViewInventory(
        RentableType types = RentableType.All,
        RentableFlag flags = RentableFlag.All)
    {
        try
        {
            var inventory = _inventoryService.GetRentables(types, flags);
            PrintRentables(inventory);
        }
        catch (VideoException ex)
        {
            ErrorHandler.HandleException(ex);
        }
    }
    public void AddMovie()
    {
        try
        {
            var genres = new HashSet<MovieGenre>(); //only one of each
            var allGenres = _inventoryService.GetAllMovieGenres();
            var choosingGenresMenu = MenuBuilder
            .CreateMenu("Choose genres")
                .OnEnter( () => Console.WriteLine("Genres added so far: "+string.Join(", ", genres)))
                .AddScreen("Add Genre", () =>
                {
                    genres.Add(
                        Chooser.ChooseAlternative<MovieGenre>("Choose Genre",
                        allGenres.Select(g => (g.Name, g)).ToArray())
                    );
                })
                .AddQuit("Continue", () => Console.WriteLine("Genres added: "+string.Join(", ", genres)));
            choosingGenresMenu.Enter();

            var movie = new Movie
            {
                Title = UserGet.GetString("Input Title"),
                Genres = genres.ToList(),
                ReleaseDate = UserGet.GetDateOnly("Release Date")
            };

            int copies = UserGet.GetInt("How many copies?");
            if (copies < 0)
                throw new VideoArgumentException("Cant add a negative amount of copies.");
                
            _inventoryService.AddMovie(movie, copies);
            Console.WriteLine(movie);
        }
        catch (VideoException ex)
        {
            ErrorHandler.HandleException(ex);
        }
    }
    public void AddGenre()
    {
        _inventoryService.AddGenre(UserGet.GetString("Genre name"));
    }
    public void AddGame()
    {
        var publishers = _inventoryService.GetAllGamePublishers();
        int copies = UserGet.GetInt("How many copies?");
        var game = _inventoryService.AddGame(new Game
            {
                Title = UserGet.GetString("Enter title"),
                ReleaseDate = UserGet.GetDateOnly("Enter release date"),
                Publisher = Chooser.ChooseAlternative("Choose publisher", publishers.Select(p => p.ToString()).ToList(), publishers)
            },
            copies
        );

        Console.WriteLine(game);
        Console.WriteLine(copies + " copies added.");
    }
    public void AddGamePublisher()
    {
        try
        {
            _inventoryService.AddPublisher(UserGet.GetString("Publisher name"));
        }
        catch (VideoException ex)
        {
            ErrorHandler.HandleException(ex);
        }
    }
    public void AddConsole()
    {
        try
        {
            var rentConsole = _inventoryService.AddRentConsole(
                UserGet.GetString("Model Name"), 
                UserGet.GetInt("How many copies?")
            );
        }
        catch (VideoException ex)
        {
            ErrorHandler.HandleException(ex);
        }
    }
    public void Search()
    {
        try
        {
            // Make user choose what types to search for
            var names = Enum.GetNames(typeof(RentableFlag)).ToList();
            var flags = Enum.GetValues<RentableFlag>().ToList();
            RentableFlag searchFlags;
            do
            {
                searchFlags = Chooser.ChooseMultiple<RentableFlag>("Choose Flags", names, flags).Aggregate((x, r) => x | r);
            } while ((int)searchFlags == 0); //  no types aren't allowed

            // Make user choose flags to include
            names = Enum.GetNames(typeof(RentableType)).ToList();
            var types = Enum.GetValues<RentableType>().ToList();
            RentableType searchTypes;
            do
            {
                searchTypes = Chooser.ChooseMultiple<RentableType>("Choose Types to search", names, types).Aggregate((x, r) => x | r);
            } while ((int)searchTypes == 0); //no flags aren't allowed

            string query = UserGet.GetString("Search");
            PrintRentables(_inventoryService.GetRentables(searchTypes, searchFlags, query));
        }
        catch (VideoException ex)
        {
            ErrorHandler.HandleException(ex);
        }
    }

    public Copy ChooseCopy() => _inventoryService.GetAvailableCopy(ChooseRentable());

    private void PrintRentables<T>(List<T> rentables) where T : Rentable =>
        Console.WriteLine(string.Join("\n", rentables.Select(i => i.ToString() + "avail cps: " + _inventoryService.GetAvailabilityOfRentable(i))));

    public Rentable ChooseRentable()
    {
        List<Rentable> rentables = new();
        MenuBuilder.CreateMenu("Find Rentable")
            .OnEnter(() => PrintRentables<Rentable>(rentables))
            .AddScreen("Search", () =>
            {
                rentables = _inventoryService.GetRentables(search: UserGet.GetString("What to find in inventory"));
            })
            .AddQuit("Pick from results")
            .AddQuit("Pick from all available", () =>
            {
                rentables = _inventoryService.GetRentables(RentableType.All, RentableFlag.In);
            })
            .Enter();

        // No customer 
        if (rentables.Count < 1)
        {
            throw new VideoArgumentException("There are no search result to pick from, first do a search, exiting.");
        }

        return Chooser.ChooseAlternative<Rentable>(
            "Pick rentable from results", 
            rentables.Select(r => 
                (r.ToString()+"avail: "+_inventoryService.GetAvailabilityOfRentable(r), r)).ToArray()
        );
    }

}