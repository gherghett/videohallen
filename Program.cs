using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using InputHandler;
using VideoHallen.Services;
using VideoHallen.EntryPoints;
using VideoHallen;
using VideoHallen.Models;

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
        .AddScreen("View All Inventory", () => inventoryEntry.ViewInventory())
        .AddMenu("Movies")
            .AddScreen("View All Movies", () => inventoryEntry.ViewInventory(RentableType.Movie))
            .AddScreen("Add Movie", inventoryEntry.AddMovie)
            .AddScreen("Add Genre", inventoryEntry.AddGenre)
            .Done()
        .AddMenu("Games")
            .AddScreen("View all Games", () => inventoryEntry.ViewInventory(RentableType.Game))
            .AddScreen("Add Game", inventoryEntry.AddGame)
            .AddScreen("Add Publisher", inventoryEntry.AddGamePublisher)
            .Done()
        .AddMenu("Console")
            .AddScreen("View all Consoles", () => inventoryEntry.ViewInventory(RentableType.RentConsole))
            .AddScreen("Add Console", inventoryEntry.AddConsole)
            .Done()
        .AddScreen("Search Inventory", inventoryEntry.Search)
        .Done()
    .AddMenu("Rentals")
        .AddScreen("View all rentals", rentingEntry.ViewAllRentals)
        .AddScreen("Inspect One Rental", rentingEntry.InspectRental)
        .AddScreen("New Rental", rentingEntry.NewRental)
        .AddScreen("New Return", rentingEntry.NewReturn)
        .Done()
    .AddQuit("Quit");

// DummyData.AddTestingData(serviceProvider);
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

