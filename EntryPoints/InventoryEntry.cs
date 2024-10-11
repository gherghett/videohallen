using VideoHallen;
using VideoHallen.Models;
using VideoHallen.Services;
using InputHandler;
using VideoHallen.Exceptions;

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
        
    public void ViewAllInventory()
    {
        try
        {
            var allInventory = _inventoryService.GetAllInventory();
            Console.WriteLine(string.Join("\n", allInventory.Select(i => i.ToString() +"available copies: "+_inventoryService.GetAvailability(i))));
        }
        catch (VideoException ex)
        {
            ErrorHandler.HandleException(ex);
        }
    }
    public void ViewAllMovies()
    {
        try
        {
            var allMovies = _inventoryService.GetAllMovies();
            Console.WriteLine(string.Join("\n", allMovies));
        }
        catch (VideoException ex)
        {
            ErrorHandler.HandleException(ex);
        }
    }
    public void ViewAllGames()
    {
        try
        {
            var allGames = _inventoryService.GetAllGames();
            Console.WriteLine(string.Join("\n", allGames));
        }
        catch (VideoException ex)
        {
            ErrorHandler.HandleException(ex);
        }
    }
    public void ViewAllConsoles()
    {
        try
        {
            var allConsoles = _inventoryService.GetAllConsoles();
            Console.WriteLine(string.Join("\n", allConsoles));
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
            var choosingGenresMenu = MenuBuilder.CreateMenu("Choose genres")
                .AddScreen("Add Genre", () =>
                {
                    genres.Add(
                        Chooser.ChooseAlternative<MovieGenre>("Choose Genre",
                        allGenres.Select(g => (g.Name, g)).ToArray())
                    );
                })
                .AddQuit("Done.");

            string title = UserGet.GetString("Input Title");
            choosingGenresMenu.Enter();
            var movie = new Movie
            {
                Title = title,
                Genres = genres.ToList()
            };
            int copies = UserGet.GetInt("How many copies?");
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
        var game = _inventoryService.AddGame(
            UserGet.GetString("Input Title"),
            UserGet.GetDateOnly("Release Date"),
            Chooser.ChooseAlternative<GamePublisher>("Choose Publisher",
                publishers.Select(p => (p.Name, p)).ToArray()
            )
        );
        int copies = UserGet.GetInt("How many copies?");
        _inventoryService.AddCopies(game, copies);
        Console.WriteLine(game);
        Console.WriteLine(copies+" copies added.");
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
        var rentConsole = _inventoryService.AddRentConsole(UserGet.GetString("Model Name"));
        int copies = UserGet.GetInt("How many copies?");
        _inventoryService.AddCopies(rentConsole, copies);
    }

    public Copy ChooseCopy() => _inventoryService.GetAvailableCopy(ChooseRentable());

    public Rentable ChooseRentable()
    {
        List<Rentable> rentables = new();
        MenuBuilder.CreateMenu("Find Rentable")
            .OnEnter(PrintSearchResults)
            .AddScreen("Search", () =>
            {
                rentables = _inventoryService.SearchAllInventory(UserGet.GetString("What to find in inventory"));
            })
            .AddQuit("Pick from results")
            .AddQuit("Pick from all",  () =>
            {
                rentables = _inventoryService.GetAllInventory();
            }) 
            .Enter();

        void PrintSearchResults()
        {
            if (rentables.Count > 0)
                Console.WriteLine(string.Join("\n", rentables));
        }

        // No customer 
        if (rentables.Count < 1)
        { 
            throw new VideoArgumentException("There are no search result to pick from, first do a search, exiting.");
        }

        return Chooser.ChooseAlternative<Rentable>("Pick rentable from results", rentables.Select(r => (r.ToString(), r)).ToArray());
    }

}