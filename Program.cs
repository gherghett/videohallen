using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using InputHandler;
using VideoHallen.Services;
using VideoHallen.Services.Pricing;
using VideoHallen.EntryPoints;
using VideoHallen;
using VideoHallen.Models;

var serviceProvider = ConfigureServices();

var customerEntry = serviceProvider.GetRequiredService<CustomerEntry>();
var inventoryEntry = serviceProvider.GetRequiredService<InventoryEntry>();
var rentingEntry = serviceProvider.GetRequiredService<RentingEntry>();

// DummyData.AddTestingData(serviceProvider);

var menu = MenuBuilder.CreateMenu("Main Menu")
    .AddMenu("Customers")
        .AddScreen("Add new customer", customerEntry.AddCustomer)
        .AddScreen("View All Customers", customerEntry.ViewAllCustomers)
        .Done()
    .AddMenu("Inventory")
        .AddScreen("View All Inventory", () => inventoryEntry.ViewInventory())
        .AddScreen("View available to rent", () => inventoryEntry.ViewInventory(RentableType.All, RentableFlag.In))
        .AddScreen("View whats rented", () => inventoryEntry.ViewInventory(RentableType.All, RentableFlag.Out))
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

menu.Enter();
// var rs = serviceProvider.GetService<RentingService>();
// var ins = serviceProvider.GetService<InventoryService>();
// List<int> copies = [
//     ins.GetAvailableCopy(1).Id,
//     ins.GetAvailableCopy(3).Id,
//     ins.GetAvailableCopy(4).Id,
// ];
// rs.CreateRental(6, copies, [3,3,3]);

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

    var simplePricing = new SimplePricingStrategy(
            moviePerDay: decimal.Parse(config["Prices:MoviePerDay"]!),
            gamePerDay: decimal.Parse(config["Prices:GamePerDay"]!),
            consolePerDay: decimal.Parse(config["Prices:ConsolePerDay"]!),
            consolePerWeek: decimal.Parse(config["Prices:ConsolePerWeek"]!)
    );

    services.AddSingleton<IPricingStrategy>(sp =>
        simplePricing
    );

    services.AddSingleton<ITotalPriceStrategy>(tft =>
        new PercentOffForStammisStrategy(
            baseStrategy: new ThreeForTwoMoviesTotalPriceStrategy(simplePricing),
            percentOff: 10
        )
    );
    
    return services.BuildServiceProvider();
}

