using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using InputHandler;
using VideoHallen.Services;
using VideoHallen.EntryPoints;
using VideoHallen;


var serviceProvider = ConfigureServices();

var customerEntry = serviceProvider.GetRequiredService<CustomerEntry>();
var inventoryEntry = serviceProvider.GetRequiredService<InventoryEntry>();
var rentingEntry = serviceProvider.GetRequiredService<RentingEntry>();


var menu = MenuBuilder.CreateMenu("Main Menu")
    .AddMenu("Customers")
        .AddScreen("Add new customer", customerEntry.AddCustomer)
        .AddScreen("View All Customers", customerEntry.ViewAllCustomers)
        .Done()
    .AddMenu("Inventory")
        .AddScreen("View All Inventory", inventoryEntry.ViewAllInventory)
        .AddMenu("Movies")
            .AddScreen("View all Movies", inventoryEntry.ViewAllMovies)
            .AddScreen("Add Movie", inventoryEntry.AddMovie)
            .AddScreen("Add Genre", inventoryEntry.AddGenre)
            .AddScreen("Search Movie", [])
            .Done()
        .AddMenu("Games")
            .AddScreen("View all Games", inventoryEntry.ViewAllGames)
            .AddScreen("Add Game", inventoryEntry.AddGame)
            .AddScreen("Add Publisher", inventoryEntry.AddGamePublisher)
            .AddScreen("Search Games", [])
            .Done()
        .AddMenu("Console")
            .AddScreen("View all Consoles", inventoryEntry.ViewAllConsoles)
            .AddScreen("Add Console", inventoryEntry.AddConsole)
            .AddScreen("Search Console", [])
            .Done()
        .AddScreen("Search Item", [])
        .AddScreen("Add Item", [])
        .Done()
    .AddMenu("Rentals")
        .AddScreen("View all rentals", rentingEntry.ViewAllRentals)
        .AddScreen("Inspect One Rental", rentingEntry.InspectRental)
        .AddScreen("New Rental", rentingEntry.NewRental)
        .AddScreen("New Return", rentingEntry.NewReturn)
        .Done()
    .AddQuit("Quit");

menu.Enter();

static ServiceProvider ConfigureServices()
{
    var services = new ServiceCollection();

    var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    services.AddSingleton<IConfiguration>(config);

    services.AddDbContext<VideoHallDbContext>(options =>
        options.UseSqlite(config.GetConnectionString("DefaultConnection")));

    services.AddScoped<CustomerService>();
    services.AddScoped<InventoryService>();
    services.AddScoped<RentingService>();

    services.AddScoped<CustomerEntry>();
    services.AddScoped<InventoryEntry>();
    services.AddScoped<RentingEntry>();

    return services.BuildServiceProvider();
}

