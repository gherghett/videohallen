using InputHandler;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using VideoHallen;
using VideoHallen.Models;
using VideoHallen.Services;

var _dbContext = new VideoHallDbContext();
var _customerService = new CustomerService(_dbContext);
var _inventoryService = new InventoryService(_dbContext);

var menu = MenuBuilder.CreateMenu("Main Menu")
    .AddMenu("Customers")
        .AddScreen("Add new customer", AddCustomer)
        .AddScreen("View All Customers", ViewAllCustomers)
        .Done()
    .AddMenu("Inventory")
        .AddScreen("View All Inventory", ViewAllInventory)
        .AddMenu("Movies")
            .AddScreen("View all Movies", ViewAllMovies)
            .AddScreen("Add Movie", AddMovie)
            .AddScreen("Add Genre", AddGenre)
            .AddScreen("Search Movie", [])
            .Done()
        .AddMenu("Games")
            .AddScreen("View all Games", ViewAllGames)
            .AddScreen("Add Game", AddGame)
            .AddScreen("Add Publisher", [])
            .AddScreen("Search Games", [])
            .Done()
        .AddMenu("Console")
            .AddScreen("View all Consoles", ViewAllConsoles)
            .AddScreen("Add Console", AddConsole)
            .AddScreen("Search Console", [])
            .Done()
        .AddScreen("Search Item", [])s
        .AddScreen("Add Item", [])
        .Done()
    .AddMenu("Rentals")
        .AddScreen("New Rental", [])
        .AddScreen("New Return", [])
        .Done()
    .AddQuit("Quit");


menu.Enter();

// AddMovie();

// ViewAllInventory();

void AddCustomer()
{
    string name = UserGet.GetString("Input new customer name");
    var customer = _customerService.AddCustomer(name);
    Console.WriteLine(customer);
}
void ViewAllCustomers()
{
    var allCustomers = _customerService.GetAllCustomers();
    Console.Write(string.Join("\n", allCustomers));
}
void ViewAllInventory()
{
    var allInventory = _inventoryService.GetAllInventory();
    Console.Write(string.Join("\n", allInventory));
}
void ViewAllMovies()
{
    var allMovies = _inventoryService.GetAllMovies();
    Console.Write(string.Join("\n", allMovies.Select(m => m + $"Available: {_inventoryService.GetAvailability(m)}")));
}
void ViewAllGames()
{
    var allGames = _inventoryService.GetAllGames();
    Console.Write(string.Join("\n", allGames));
}
void ViewAllConsoles()
{
    var allConsoles = _inventoryService.GetAllConsoles();
    Console.WriteLine(string.Join("\n", allConsoles));
}
void AddMovie()
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
void AddGenre()
{
    _inventoryService.AddGenre(UserGet.GetString("Genre name"));
}
void AddGame()
{
    var publishers = _inventoryService.GetAllGamePublishers();
    var game = _inventoryService.AddGame(
        UserGet.GetString("Input Title"),
        UserGet.GetDateOnly("Release Date"),
        Chooser.ChooseAlternative<GamePublisher>("Choose Publisher",
             publishers.Select(p => (p.Name, p)).ToArray()
        )
    );
    Console.WriteLine(game);
}
void AddConsole()
{
    _inventoryService.AddRentConsole(UserGet.GetString("Model Name"));
}